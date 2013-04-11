using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CardboardBox.Nerwork;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using libDanbooru2;

namespace CardboardBox
{
    public partial class HomePage : PhoneApplicationPage
    {
        #region App Bar Buttons

        private readonly ApplicationBarIconButton[] whatsNewAppbarButtons = new ApplicationBarIconButton[]
            {
                new ApplicationBarIconButton(), 
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

            Loaded += (@s, e) =>
                {
                    // Remove login screen from back stack
                    if (NavigationService.BackStack.Any())
                        NavigationService.RemoveBackEntry();
                };
        }

        private void AttachWhatsNewHandlers()
        {
            
        }

        private void AttachAppBarHandlers()
        {
            // Attach App Bar Commands
            ((ApplicationBarMenuItem) ApplicationBar.MenuItems[0]).Click += (@s, e) => Session.Instance.LogOut();

            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Click += (@s, e) =>
                {
                    NavigationService.Navigate(new Uri("/ViewPost.xaml", UriKind.Relative));
                };
            
        }

    }
}