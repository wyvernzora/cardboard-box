using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using CardboardBox.UI;
using CardboardBox.Utilities;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using libWyvernzora.Core;

namespace CardboardBox
{
    public partial class SearchPage : PhoneApplicationPage
    {
        #region Constants

        private const Int32 PageLoadThreshold = 60; 
        
        #endregion

        #region Search Results, Monitor and Flags

        public PostTupleCollection SearchResults 
        { get; set; }


        private ScrollViewMonitor monitor;
        private Int32 resultPage;
        private Boolean resultsLoading;
        private Boolean resultReachedEnd;

        private String[] query;

        #endregion
        
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

            // Attach Loaded Event
            Loaded += (@s, e) => PageLoaded();

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

            // Attach Events
            SearchBox.TextChanged += (@s, e) => UpdateSearchMessage();

            // Initialize Search Message
            UpdateSearchMessage();
        }

        private void PageLoaded()
        {
            SearchBox.ActionIconTapped += (@s, e) =>
                                          ExecuteSearch(SearchBox.Text);
            SearchBox.KeyDown += (@s, e) =>
                {
                    if (e.Key == Key.Enter)
                        ExecuteSearch(SearchBox.Text);
                };
        }

        /// <summary>
        /// Loads more search results silently.
        /// </summary>
        /// <param name="silent">If true, UI state will not be altered.</param>
        private void LoadNextPage(Boolean silent = true)
        {
            if (query == null)
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
                PostTuple[] tuples = Session.Instance.ExecutePostQuery(resultPage++, 1, query);

                if (tuples.Length < Session.PostRequestTupleSize)
                {
                    Logging.D("Search feed reached its end!");
                    resultReachedEnd = true;
                }

                Dispatcher.BeginInvoke(() =>
                {
                    tuples.ForEach(v => SearchResults.Add(v));
                    resultsLoading = false;
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
        /// Updates the tag count so that it becomes red if user attempts to
        /// search too many tags.
        /// </summary>
        private void UpdateSearchMessage()
        {
            Int32 levelLimit = Session.Instance.Level.TagLimit -
                               (String.IsNullOrEmpty(Session.Instance.MaxComplement) ? 0 : 1);
            String[] tags = SearchBox.Text.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            if (tags.Any(t => t.StartsWith("rating:", StringComparison.CurrentCultureIgnoreCase))
                   && !String.IsNullOrEmpty(Session.Instance.MaxComplement))
                levelLimit += 1;    // Detect rating overrides

            SearchMessage.Text = String.Format("{0} / {1} Tags", tags.Length, levelLimit);

            VisualStateManager.GoToState(this, tags.Length > levelLimit ? "TagOverflow" : "SearchValid", true);
        }

        /// <summary>
        /// Attempts to execute the query string.
        /// </summary>
        /// <param name="q">Query string</param>
        /// <returns>True if successful; false otherwise.</returns>
        private Boolean ExecuteSearch(String q)
        {
            // Get user settings first
            String rating = Session.Instance.MaxComplement;
            Boolean hasRating = !String.IsNullOrEmpty(rating);
            Int32 levelLimit = Session.Instance.Level.TagLimit;

            // Get tags from search
            List<String> tags = new List<String>(SearchBox.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            // Check for rating filter, it uses 1 tag
            if (hasRating && !tags.Any(t => t.StartsWith("rating:", StringComparison.CurrentCultureIgnoreCase)))
                levelLimit--;

            // Check for overflow
            if (tags.Count > levelLimit)
                return false;

            // Clear current results
            SearchResults.Clear();
            resultPage = 1;
            resultsLoading = false;
            resultReachedEnd = false;

            // Ask for next page
            query = tags.ToArray();
            LoadNextPage(false);

            // Return success
            return true;
        }
    }
}