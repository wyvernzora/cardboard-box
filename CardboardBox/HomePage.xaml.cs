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
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using libDanbooru2;

namespace CardboardBox
{
    public partial class HomePage : PhoneApplicationPage
    {
        public HomePage()
        {
            InitializeComponent();

            // Load Appropriate Banner
            String bannerUrl = App.IsInDarkTheme() ? "/Assets/banner-dark.png" : "/Assets/banner-light.png";
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

            Panorama.SelectionChanged += (@s, e) =>
                {
                    switch (Panorama.SelectedIndex)
                    {
                        case 0:     // What's New
                            
                            ApplicationBar.IsVisible = true;
                            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = true;
                            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = false;

                            break;
                        case 1:     // Favorite

                            ApplicationBar.IsVisible = false;

                            break;
                        case 2:     // Subscriptions

                            ApplicationBar.IsVisible = false;
                            

                            break;
                        case 3:     // User Profile

                            ApplicationBar.IsVisible = true;
                            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = false;
                            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = true;

                            break;
                    }
                };

        }

    }
}