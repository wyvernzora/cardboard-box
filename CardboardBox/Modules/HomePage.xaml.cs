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
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using libDanbooru2;
using libWyvernzora.Utilities;

namespace CardboardBox
{
    public partial class HomePage : PhoneApplicationPage
    {
        #region Constants

        private const Int32 PostPageLoadingThreshold = 60;

        #endregion

        #region App Bar Buttons

        private readonly ApplicationBarIconButton[] whatsNewAppbarButtons = new ApplicationBarIconButton[]
            {
                new ApplicationBarIconButton{IconUri = new Uri("/Assets/SDK/feature.search.png",UriKind.Relative), Text = "search "}
            };

        private readonly ApplicationBarIconButton[] favoriteAppbarButtons = new ApplicationBarIconButton[]
            {

            };

        private readonly ApplicationBarIconButton[] subscriptionAppbarButtons = new ApplicationBarIconButton[]
            {
                new ApplicationBarIconButton {IconUri = new Uri("/Assets/SDK/add.png", UriKind.Relative), Text = "add new"},
                new ApplicationBarIconButton {IconUri = new Uri("/Assets/SDK/refresh.png", UriKind.Relative), Text = "refresh"} 
            };

        private readonly ApplicationBarIconButton[] profileAppbarButtons = new ApplicationBarIconButton[]
            {
                 
            };


        #endregion

        public HomePage()
        {
            InitializeComponent();

            // Load Appropriate Banner
            String bannerUrl = App.IsInDarkTheme() 
                ? "/Assets/application-banner-dark.png"
                : "/Assets/application-banner-light.png";
            BitmapImage bannerSrc = new BitmapImage(new Uri(bannerUrl, UriKind.Relative));
            Panorama.Title = bannerSrc;

            // Initialize User Profile Page
            UsernameTextBlock.Text = Session.Instance.User.Name;
            UserLevelTextBlock.Text = Session.Instance.GetUserLevel(Session.Instance.User.Level).Name;
            JoinDateTextBlock.Text = Session.Instance.User.CreatedAt.ToLongDateString();
            UserIdTextBlock.Text = Session.Instance.User.ID.ToString(CultureInfo.InvariantCulture);



            // Attach Event Handlers
            AttachAppBarHandlers();
            AttachWhatsNewHandlers();

            // Load Initial Posts
            NewPostList.ItemsSource = NewPosts;

            // Add New Post Entries
            Loaded += (@s, e) =>
                {
                    // Remove login screen from back stack
                    while (NavigationService.BackStack.Any())
                        NavigationService.RemoveBackEntry();

                    // Attach Scroll Monitors
                    newPostListMonitor = new ScrollViewMonitor(NewPostList);
                    newPostListMonitor.Scroll += (@o, a) =>
                        {
                            if (a.OffsetY > a.MaxY - PostPageLoadingThreshold)
                                LoadNextNewPostPage();
                        };

                    // Add AppBar Button
                    if (ApplicationBar.Buttons.Count == 0)
                        UpdateAppBarButtons();
                        
                };
        }

        #region Whats New

        private ScrollViewMonitor newPostListMonitor;
        private Boolean newPostsLoading;
        private Boolean reachedEnd = false;


        public PostTupleCollection NewPosts 
        { get { return Session.Instance.NewPosts; } }

        private void AttachWhatsNewHandlers()
        {
            // Attach Handlers
            NewPostList.Tap += (@s, e) =>
                {
                    var p = NewPostList.SelectedItem as PostTuple;
                    if (p == null) return;

                    Int32 row = (Int32)Math.Floor(e.GetPosition(NewPostList).X / 130);

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
                        Session.Instance.Navigate(new Uri("/Modules/ViewPost.xaml", UriKind.Relative));
                };
        }

        private void LoadNextNewPostPage()
        {
            if (newPostsLoading)
                return;

            if (reachedEnd)
            {
                Logging.D("Cannot load more new posts: end of feed.");
                return;
            }

            newPostsLoading = true;

            ThreadPool.QueueUserWorkItem(@a =>
                {
                    PostTuple[] tuples = Session.Instance.GetMoreNewPosts(1);

                    if (tuples.Length < Session.PostRequestTupleSize)
                    {
                        Logging.D("New post feed reached the end!");
                        reachedEnd = true;
                    }

                    Dispatcher.BeginInvoke(() =>
                        {
                            foreach (var t in tuples)
                                NewPosts.Add(t);

                            newPostsLoading = false;

                        });
                });

        }

        #endregion
        
        private void AttachAppBarHandlers()
        {
            // Attach App Bar Commands
            ((ApplicationBarMenuItem) ApplicationBar.MenuItems[1]).Click += (@s, e) => Session.Instance.LogOut();

            Panorama.SelectionChanged += (@s, e) => UpdateAppBarButtons();

            // Whats New App Bar Buttons
            whatsNewAppbarButtons[0].Click += (@s, e) => Session.Instance.Navigate(new Uri("/Modules/SearchPage.xaml", UriKind.Relative));

        }

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