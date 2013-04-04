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

namespace CardboardBox
{
    public partial class HomePage : PhoneApplicationPage
    {
        public HomePage()
        {
            InitializeComponent();

            // Load Appropriate Banner
            String bannerUrl = App.IsInDarkTheme() ? "/CardboardBox;component/Assets/panorama-background-dark.png" : "/CardboardBox;component/Assets/panorama-background-light.png";
            BitmapImage bannerSrc = new BitmapImage(new Uri(bannerUrl, UriKind.Relative));
            Panorama.Background = new ImageBrush() {ImageSource = bannerSrc};


            Loaded += (@s, e) =>
                {
                    // Remove login screen from back stack
                    if (NavigationService.BackStack.Any())
                        NavigationService.RemoveBackEntry();

                    // Add What's new Items
                    for (int i = 0; i < 60; i++)
                    {
                        var post = CardboardBoxSession.Instance.WhatsNewPosts[i];
                        using (
                            IsolatedStorageFileStream fs =
                                new IsolatedStorageFileStream(Constants.TileCachePath + post.ID.ToString(CultureInfo.InvariantCulture) + ".jpg",
                                                              FileMode.Open, CardboardBoxSession.Instance.IsolatedStorage))
                        {
                            BitmapImage img = new BitmapImage();
                            img.SetSource(fs);
                            post.ImageSource = img;
                        }

                        ListBoxWhatsNew.Items.Add(post);
                    }

                };
        }
    }
}