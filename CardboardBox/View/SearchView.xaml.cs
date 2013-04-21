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
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CardboardBox.UI;
using CardboardBox.Utilities;
using CardboardBox.ViewModel;
using Microsoft.Phone.Shell;

namespace CardboardBox
{
    public partial class SearchView
    {
        private readonly SearchViewModel viewModel;
        private ListViewInfinityDaemon infinityDaemon;

        /// <summary>
        ///     Constructor.
        ///     Everyone know what a constructor does.
        /// </summary>
        public SearchView()
        {
            InitializeComponent();

            // Create ViewModel
            viewModel = new SearchViewModel(this);
            viewModel.ChangeState += (@s, e) =>
                {
                    if (SystemTray.ProgressIndicator != null)
                        SystemTray.ProgressIndicator.IsVisible = e.State == SearchViewModel.LoadingState;
                    if (e.State == HomeViewModel.LogoutState)
                    {
                        if (
                            MessageBox.Show("Are you sure you want to log out?", "O!Pix Log Out",
                                            MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                            Session.Instance.LogOut();
                    }
                    else
                        VisualStateManager.GoToState(this, e.State, e.Transition);
                };
            DataContext = viewModel;

            // Load Appropriate Logo
            String logoUrl = App.IsInDarkTheme() ? "/Assets/application-banner-small-dark.png" : "/Assets/application-banner-small-light.png";
            var logoSrc = new BitmapImage(new Uri(logoUrl, UriKind.Relative));
            AppLogoImage.Source = logoSrc;

            // Create Infinity Daemon
            Loaded += (@s, e) => PageLoaded();

            // Attach Event Handlers (since there is no command support)
            SearchBox.ActionIconTapped += (@s, e) =>
            {
                Focus();
                viewModel.SearchCommand.Execute(null);
            };
            SearchBox.KeyDown += (@s, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    Focus();
                    viewModel.SearchCommand.Execute(null);
                }
            };
            SearchBox.TextChanged += (@s, e) =>
                {
                    UpdateSearchMessage();
                    viewModel.SearchString = SearchBox.Text;
                };

            UpdateSearchMessage();
        }

        /// <summary>
        ///     Initialize items that require the page to be
        ///     completely loaded.
        /// </summary>
        private void PageLoaded()
        {
            PostTuple.SetViewCommand(viewModel.ViewCommand);

            if (ApplicationBar.Buttons.Count == 0)
                ApplicationBar.Buttons.Add(new AppBarButton
                    {
                        IconUri = new Uri("/Assets/SDK/favs.addto.png", UriKind.Relative),
                        Text = "bookmark",
                        Command = viewModel.BookmarkCommand
                    });

            if (ApplicationBar.MenuItems.Count == 0)
            {
                ApplicationBar.MenuItems.Add(new AppBarMenuItem
                    {
                        Text = "settings",
                        Command = viewModel.SettingsCommand
                    });
                ApplicationBar.MenuItems.Add(new AppBarMenuItem
                {
                    Text = "logout",
                    Command = viewModel.LogoutCommand
                });
            }

            infinityDaemon = new ListViewInfinityDaemon(SearchResultList, 60, viewModel.LoadPageCommand);
            PostTuple.SetViewCommand(viewModel.ViewCommand);

            // Load search results if not already searching
            if (NavigationHelper.NavigationArgument != null)
            {
                String query = NavigationHelper.NavigationArgument as String;
                if (query != null)
                {
                    query = query.Replace("+", " ");
                    viewModel.SearchString = query;
                    SearchBox.Text = query;
                    Focus();
                    viewModel.SearchCommand.Execute(null);
                }
            }
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
    }
}