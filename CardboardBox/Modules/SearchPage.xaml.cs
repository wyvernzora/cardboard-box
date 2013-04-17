// ReSharper disable CheckNamespace

// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// CardboardBox/SearchPage.xaml.cs
// --------------------------------------------------------------------------------
// Copyright (c) 2013, Jieni Luchijinzhou a.k.a Aragorn Wyvernzora
// 
// This file is a part of CardboardBox.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
// of the Software, and to permit persons to whom the Software is furnished to do 
// so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CardboardBox.UI;
using CardboardBox.Utilities;
using Microsoft.Phone.Shell;
using libWyvernzora.Core;

namespace CardboardBox
{
    public partial class SearchPage
    {
        #region Constants

        // Threshold to load the next result page
        // larger threshold results in earlier page loads
        // can be useful for countering slow network speeds
        private const Int32 PageLoadThreshold = 60;

        #endregion

        #region Page variables

        private Boolean eventsAttached;

        private ScrollViewMonitor monitor;
        private Int32 resultPage;
        private Boolean resultReachedEnd;
        private Boolean resultsLoading;
        public PostTupleCollection SearchResults { get; set; }

        #endregion

        /// <summary>
        ///     Constructor.
        ///     Everyone know what a constructor does.
        /// </summary>
        public SearchPage()
        {
            InitializeComponent();

            // Initialize Stuff
            SearchResults = new PostTupleCollection();
            resultPage = 1;
            resultsLoading = false;
            resultReachedEnd = false;

            // Attach Items Source
            SearchResultList.ItemsSource = SearchResults;

            // Load Appropriate Logo
            String logoUrl = App.IsInDarkTheme() ? "/Assets/banner-dark-alt.png" : "/Assets/banner-light-alt.png";
            var logoSrc = new BitmapImage(new Uri(logoUrl, UriKind.Relative));
            AppLogoImage.Source = logoSrc;

            // Attach Event Handlers
            AttachEventHandlers();

            // Initialize Search Message
            UpdateSearchMessage();
        }

        /// <summary>
        ///     Initialize items that require the page to be
        ///     completely loaded.
        /// </summary>
        private void PageLoaded()
        {
            // Attach App Bar Event Handlers
            if (!eventsAttached)
            {
                ApplicationBarIconButton addBookmarkButton = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
                if (addBookmarkButton != null)
                    addBookmarkButton.Click += (@s, e) => MessageBox.Show("Feature not implemented.");

                ApplicationBarMenuItem settingsMenuItem = ApplicationBar.MenuItems[0] as ApplicationBarMenuItem;
                if (settingsMenuItem != null)
                    settingsMenuItem.Click += (@s, e) => MessageBox.Show("Feature not implemented.");

                ApplicationBarMenuItem logOutMenuItem = ApplicationBar.MenuItems[1] as ApplicationBarMenuItem;
                if (logOutMenuItem != null)
                    logOutMenuItem.Click += (@s, e) =>
                        {
                            if (
                                MessageBox.Show("Are you sure that you want to log out?", "Opix Logout",
                                                MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                Session.Instance.LogOut();
                        };

                eventsAttached = true;
            }

            // Load search results if not already searching
            String query;
            if (NavigationContext.QueryString.TryGetValue("query", out query))
            {
                query = query.Replace("+", " ");
                SearchBox.Text = query;
                Focus();
                ExecuteSearch(query);
            }
        }

        /// <summary>
        ///     Attaches event handlers and
        /// </summary>
        private void AttachEventHandlers()
        {
            // Attach Loaded Event
            Loaded += (@s, e) => PageLoaded();

            // Attach Event Handlers
            SearchBox.ActionIconTapped += (@s, e) =>
                {
                    Focus();
                    ExecuteSearch(SearchBox.Text);
                };
            SearchBox.KeyDown += (@s, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        Focus();
                        ExecuteSearch(SearchBox.Text);
                    }
                };
            SearchResultList.Tap += (@s, e) =>
                {
                    PostTuple p = SearchResultList.SelectedItem as PostTuple;
                    if (p == null) return;

                    Int32 row = (Int32) Math.Floor(e.GetPosition(SearchResultList).X / 130);

                    switch (row)
                    {
                        case 0:
                            Session.Instance.Selected = p.First;
                            break;
                        case 1:
                            Session.Instance.Selected = p.Second;
                            break;
                        case 2:
                            Session.Instance.Selected = p.Third;
                            break;
                        default:
                            return;
                    }

                    if (Session.Instance.Selected != null)
                        NavigationService.Navigate(new Uri("/Modules/ViewPost.xaml", UriKind.Relative));
                };

            // Set up ScrollMonitor
            SearchResultList.Loaded += (@s, e) =>
                {
                    monitor = new ScrollViewMonitor(SearchResultList);
                    monitor.Scroll += (@o, a) =>
                        {
                            if (a.OffsetY > a.MaxY - PageLoadThreshold)
                                LoadNextPage();
                        };
                };

            SearchBox.TextChanged += (@s, e) => UpdateSearchMessage();
        }

        /// <summary>
        ///     Loads more search results silently.
        /// </summary>
        /// <param name="silent">If true, UI state will not be altered.</param>
        private void LoadNextPage(Boolean silent = true)
        {
            if (Session.Instance.Query == null)
                return;

            if (resultsLoading)
                return;

            if (resultReachedEnd)
            {
                Logging.D("Search results at the end, server request skipped.");
                return;
            }

            resultsLoading = true;
            if (!silent)
            {
                if (SystemTray.ProgressIndicator != null)
                {
                    SystemTray.ProgressIndicator.IsVisible = true;
                    SystemTray.ProgressIndicator.IsIndeterminate = true;
                }
                VisualStateManager.GoToState(this, "DataLoading", true);
            }

            ThreadPool.QueueUserWorkItem(a =>
                {
                    PostTuple[] tuples = Session.Instance.ExecutePostQuery(resultPage++, 1, Session.Instance.Query);

                    if (tuples.Length < Session.PostRequestTupleSize)
                    {
                        Logging.D("Search feed reached its end!");
                        resultReachedEnd = true;
                    }

                    Dispatcher.BeginInvoke(() =>
                        {
                            tuples.ForEach(v => SearchResults.Add(v));
                            resultsLoading = false;

                            // Show "no result" message if result collection is empty
                            NoResultBox.Visibility = SearchResults.Count == 0
                                                         ? Visibility.Visible
                                                         : Visibility.Collapsed;

                            // Go back to normal visual state
                            if (!silent)
                            {
                                if (SystemTray.ProgressIndicator != null)
                                    SystemTray.ProgressIndicator.IsVisible = false;
                                VisualStateManager.GoToState(this, "DataLoaded", true);
                            }
                        });
                });
        }

        /// <summary>
        ///     Updates the tag count so that it becomes red if user attempts to
        ///     search too many tags.
        /// </summary>
        private void UpdateSearchMessage()
        {
            Int32 levelLimit = Session.Instance.Level.TagLimit -
                               (String.IsNullOrEmpty(Session.Instance.MaxComplement) ? 0 : 1);
            String[] tags = SearchBox.Text.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            if (tags.Any(t => t.StartsWith("rating:", StringComparison.CurrentCultureIgnoreCase))
                && !String.IsNullOrEmpty(Session.Instance.MaxComplement))
                levelLimit += 1; // Detect rating overrides

            SearchMessage.Text = String.Format("{0} / {1} Tags", tags.Length, levelLimit);

            VisualStateManager.GoToState(this, tags.Length > levelLimit ? "TagOverflow" : "SearchValid", true);
        }

        /// <summary>
        ///     Attempts to execute the query string.
        /// </summary>
        /// <param name="q">Query string</param>
        /// <returns>True if successful; false otherwise.</returns>
        private void ExecuteSearch(String q)
        {
            // Get user settings first
            String rating = Session.Instance.MaxComplement;
            Boolean hasRating = !String.IsNullOrEmpty(rating);
            Int32 levelLimit = Session.Instance.Level.TagLimit;

            // Get tags from search
            String[] tags = Session.Instance.ResolveQueryString(SearchBox.Text);

            // Check for overflow
            if (tags.Length > levelLimit)
            {
                return;
            }


            // Clear current results
            SearchResults.Clear();
            resultPage = 1;
            resultsLoading = false;
            resultReachedEnd = false;

            // Ask for next page
            Session.Instance.Query = tags.ToArray();
            LoadNextPage(false);
        }
    }
}