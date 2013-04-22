using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CardboardBox.UI;
using CardboardBox.Utilities;
using CardboardBox.ViewModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using libDanbooru2;
using libWyvernzora.Utilities;

namespace CardboardBox
{
    public partial class HomeView
    {
        private readonly HomeViewModel viewModel;

        #region App Bar Buttons

        private readonly ApplicationBarIconButton[] whatsNewAppbarButtons;
        private readonly ApplicationBarIconButton[] favoriteAppbarButtons;
        private readonly ApplicationBarIconButton[] subscriptionAppbarButtons;
        private readonly ApplicationBarIconButton[] profileAppbarButtons;

        #endregion

        public HomeView()
        {
            InitializeComponent();

            // Create ViewModel
            viewModel = new HomeViewModel(this);
            viewModel.ChangeState += (@s, e) => {
                if (e.State == HomeViewModel.LogoutState)
                {
                    if (MessageBox.Show("Are you sure you want to log out?", "O!Pix Log Out", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        Session.Instance.LogOut();
                }
                else if (e.State == HomeViewModel.FavChangedState)
                {
                    // Show or hide the "No Favorite" prompt
                    NoFavTextBlock.Visibility = viewModel.Favorites.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                }else
                    VisualStateManager.GoToState(this, e.State, e.Transition);
            };
            DataContext = viewModel;

            // Load Appropriate Banner
            String bannerUrl = App.IsInDarkTheme()
                                   ? "/Assets/application-banner-dark.png"
                                   : "/Assets/application-banner-light.png";
            BitmapImage bannerSrc = new BitmapImage(new Uri(bannerUrl, UriKind.Relative));
            Panorama.Title = bannerSrc;

            // Page Loaded Handler
            Loaded += (@s, e) => PageLoaded();

            // App Bar Handler
            Panorama.SelectionChanged += (@s, e) => UpdateAppBarButtons();

            // Initialize Infinity Daemons
            NewPostList.Loaded += (@s, e) =>
                { newPostInfinityDaemon = new ListViewInfinityDaemon(NewPostList, 60, viewModel.LoadNewPostsCommand); };
            FavoriteList.Loaded += (@s, e) =>
                {
                    favPostInfinityDaemon = new ListViewInfinityDaemon(FavoriteList, 60,
                                                                       viewModel.LoadFavoritePostsCommand);
                };

            // Set up App Bar Buttons
            whatsNewAppbarButtons = new ApplicationBarIconButton[]
                {
                    new AppBarButton { IconUri = new Uri("/Assets/SDK/feature.search.png", UriKind.Relative), Text = "search", Command = viewModel.SearchCommand }
                };

            favoriteAppbarButtons = new ApplicationBarIconButton[]
                {

                };

            subscriptionAppbarButtons = new ApplicationBarIconButton[]
                {
                    new AppBarButton { IconUri = new Uri("/Assets/SDK/add.png", UriKind.Relative), Text = "add new", Command = viewModel.SearchCommand },
                    new AppBarButton { IconUri = new Uri("/Assets/SDK/refresh.png", UriKind.Relative), Text = "refresh", Command = viewModel.SearchCommand }
                };

            profileAppbarButtons = new ApplicationBarIconButton[]
                {

                };
        }

        private void PageLoaded()
        {
            PostTuple.SetViewCommand(viewModel.ViewCommand);

            // Reload View Model
            viewModel.PageLoadedCommand.Execute(null);

            // Remove login screen from back stack
            while (NavigationService.BackStack.Any())
                NavigationService.RemoveBackEntry();

            // Add AppBar Button
            if (ApplicationBar.Buttons.Count == 0)
                UpdateAppBarButtons();

            // Set Up App Bar Menu Items
            if (ApplicationBar.MenuItems.Count == 0)
            {
                ApplicationBar.MenuItems.Add(new AppBarMenuItem {Text = "settings", Command = viewModel.SettingsCommand});
                ApplicationBar.MenuItems.Add(new AppBarMenuItem {Text = "log out", Command = viewModel.LogoutCommand});
            }

            

        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (
                MessageBox.Show("Are you sure you want to close O!Pix?", "Exit O!Pix?",
                                MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                e.Cancel = true;
        }

        #region Whats New

        private ListViewInfinityDaemon newPostInfinityDaemon;

        #endregion

        #region Favorites

        private ListViewInfinityDaemon favPostInfinityDaemon;

        #endregion

        private void UpdateAppBarButtons()
        {
            ApplicationBar.Buttons.Clear();

            switch (Panorama.SelectedIndex)
            {
                case 0:
                    ApplicationBar.Mode = ApplicationBarMode.Default;
                    foreach (var v in whatsNewAppbarButtons)
                        ApplicationBar.Buttons.Add(v);
                    break;
                case 1:
                    ApplicationBar.Mode = ApplicationBarMode.Minimized;
                    foreach (var v in favoriteAppbarButtons)
                        ApplicationBar.Buttons.Add(v);
                    break;
                case 2:
                    ApplicationBar.Mode = ApplicationBarMode.Default;
                    foreach (var v in subscriptionAppbarButtons)
                        ApplicationBar.Buttons.Add(v);
                    break;
                case 3:
                    ApplicationBar.Mode = ApplicationBarMode.Default;
                    foreach (var v in profileAppbarButtons)
                        ApplicationBar.Buttons.Add(v);
                    break;
            }
        }

    }
}