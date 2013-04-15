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
using CardboardBox.Nerwork;
using CardboardBox.UI;
using CardboardBox.Utilities;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using libDanbooru2;
using libDanbooru2.Utilities;
using libWyvernzora.Utilities;

namespace CardboardBox
{
    public partial class HomePage : PhoneApplicationPage
    {
        #region Constants

        private const Int32 PostPageLoadingThreshold = 240;

        private const Int32 PostPageSize = 10; // Rows

        #endregion

        #region ListBox Monitors

        private ScrollViewMonitor newPostListMonitor;
        private Boolean newPostsLoading;

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
                new ApplicationBarIconButton {IconUri = new Uri("/Assets/SDK/", UriKind.Relative)} 
            };

        private readonly ApplicationBarIconButton[] profileAppbarButtons = new ApplicationBarIconButton[]
            {
                 
            };


        #endregion

        public HomePage()
        {
            InitializeComponent();

            // Load Appropriate Banner
            String bannerUrl = App.IsInDarkTheme() ? "/Assets/banner-dark-alt.png" : "/Assets/banner-light-alt.png";
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
          //  foreach (var p in Session.Instance.NewPosts)
          //      NewPostList.Items.Add(p);

            Loaded += (@s, e) =>
                {
                    // Remove login screen from back stack
                    if (NavigationService.BackStack.Any())
                        NavigationService.RemoveBackEntry();

                    // Attach Scroll Monitors
                    newPostListMonitor = new ScrollViewMonitor(NewPostList);

                    newPostListMonitor.Scroll += (@o, a) =>
                        {
                            if (a.OffsetY > a.MaxY - PostPageLoadingThreshold)
                                LoadNextNewPostPage();
                        };

                };
        }

        #region Whats New

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

                    NavigationService.Navigate(new Uri("/ViewPost.xaml", UriKind.Relative));
                };
        }

        private void LoadNextNewPostPage()
        {
            if (newPostsLoading)
                return;

            newPostsLoading = true;

            ThreadPool.QueueUserWorkItem(@a =>
                {
                    PostTuple[] tuples = Session.Instance.GetMoreNewPosts(1);

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
            
        }



    }
}