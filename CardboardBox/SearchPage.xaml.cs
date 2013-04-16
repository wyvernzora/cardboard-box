using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using libDanbooru2;

namespace CardboardBox
{
    public partial class SearchPage : PhoneApplicationPage
    {
        public SearchPage()
        {
            InitializeComponent();

            // Load appropriate search button image
           /* if (App.IsInDarkTheme())
            {
                SearchButton.Background = new ImageBrush()
                    {
                        ImageSource =
                            (ImageSource) new ImageSourceConverter().ConvertFromString("/Assets/SDK/basecircle.png")
                    };
                SearchButtonImage.Source =
                    (ImageSource) new ImageSourceConverter().ConvertFromString("/Assets/SDK/feature.search.png");
            }
            else
            {
                SearchButton.Background = new ImageBrush()
                {
                    ImageSource =
                        (ImageSource)new ImageSourceConverter().ConvertFromString("/Assets/SDK/basecircle-light.png")
                };
                SearchButtonImage.Source =
                    (ImageSource)new ImageSourceConverter().ConvertFromString("/Assets/SDK/feature.search-light.png");
            }
            */

            // Initialize Search Message
            String searchMessage = String.Format("Your user level allows {0} tags per search.",
                                                 Session.Instance.GetUserLevel(Session.Instance.User.Level).TagLimit);
            if (Session.Instance.MaxRating != Rating.Explicit)
                searchMessage += "\nRating filter enabled: it uses 1 tag per search.";
            SearchMessage.Text = searchMessage;
        }
    }
}