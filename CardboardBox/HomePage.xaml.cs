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
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using libDanbooru2;
using libWyvernzora.Utilities;

namespace CardboardBox
{
    public partial class HomePage : PhoneApplicationPage
    {
        #region Constants

        private const Int32 PostPageLoadingThreshold = 640;

        private const Int32 PostPageSize = 30;

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
            // Add New Post Entries
            foreach (var p in Session.Instance.NewPosts)
                NewPostList.Items.Add(p);

            Loaded += (@s, e) =>
                {
                    // Remove login screen from back stack
                    if (NavigationService.BackStack.Any())
                        NavigationService.RemoveBackEntry();

                    // Attach Scroll Monitors
                    newPostListMonitor = new ScrollViewMonitor(NewPostList);

                    newPostListMonitor.Scroll += (@o, a) =>
                    {
                        WhatsNewPanoramaItem.Header = a.OffsetY;
                        if (a.OffsetY > a.MaxY - PostPageLoadingThreshold)
                            LoadNextNewPostPage();
                    };

                };
        }

        #region Whats New

        private void AttachWhatsNewHandlers()
        {            
            
            // Attach Handlers
            NewPostList.Tap += (@s, e) =>
                {
                    Post p = NewPostList.SelectedItem as Post;
                    if (p == null) return;
                    Session.Instance.Selected = p;
                    NavigationService.Navigate(new Uri("/ViewPost.xaml", UriKind.Relative));
                };
        }

        private void LoadNextNewPostPage()
        {
            if (newPostsLoading)
                return;

            newPostsLoading = true;
            Int32 count = NewPostList.Items.Count;
          //  SystemTray.ProgressIndicator.IsVisible = true;
           // SystemTray.ProgressIndicator.IsIndeterminate = true;

            ThreadPool.QueueUserWorkItem(@a =>
                {
                    if (Session.Instance.NewPosts.Count - count <= PostPageSize)
                    {
                        var request = new DanbooruRequest<Post[]>(Session.Instance.Credentials,
                                                                  Session.SiteUrl + Session.PostIndexUrl);

                        if (Session.Instance.MaxRating == Rating.Safe) request.AddArgument("tag", "rating:s");
                        else if (Session.Instance.MaxRating == Rating.Questionable) request.AddArgument("tag", "-rating:e");

                        request.AddArgument("limit", Session.PostRequestPageSize);
                        request.AddArgument("page", count / PostPageSize + 1);

                        request.ExecuteRequest(Session.Instance.Cookie);
                        while (request.Status == -1) ; // Wait

                        Post[] result = request.Result;
                        if (result == null)
                            return;
                        foreach (var p in result)
                            p.PreviewUrl = new Uri(Session.SiteUrl + Session.PreviewDir + p.MD5 + ".jpg");

                        Session.Instance.NewPosts.AddRange(result);
                    }

                    Dispatcher.BeginInvoke(() =>
                        {
                            for (int i = count; i < count + PostPageSize; i++)
                                NewPostList.Items.Add(Session.Instance.NewPosts[i]);

                            newPostsLoading = false;

                 //           SystemTray.ProgressIndicator.IsVisible = false;
               //             SystemTray.ProgressIndicator.IsIndeterminate = false;
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