using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Media.Imaging;
using libWyvernzora.BarlogX.Animation;
using System.Windows.Resources;

namespace CardboardBox
{
    public partial class SettingsView
    {
        public SettingsView()
        {
            InitializeComponent();
            // Load Appropriate Banner
            String bannerUrl = App.IsInDarkTheme()
                                   ? "/Assets/application-banner-dark.png"
                                   : "/Assets/application-banner-light.png";
            BitmapImage bannerSrc = new BitmapImage(new Uri(bannerUrl, UriKind.Relative));
            BannerImage.Source = bannerSrc;
            InitializeAnimation();

            RatingListPicker.SelectionChanged += (@s, e) =>
                {
                    //ListPickerItem item = RatingListPicker.SelectedItem as ListPickerItem;
                    Int32 index = RatingListPicker.SelectedIndex;

                    switch (index)
                    {
                        case 0:

                            Session.Instance.MaxRating = libDanbooru2.Rating.Safe;

                            break;
                        case 1:

                            Session.Instance.MaxRating = libDanbooru2.Rating.Questionable;

                            break;
                        case 2:

                            Session.Instance.MaxRating = libDanbooru2.Rating.Explicit;

                            break;
                    }
                    
                };
        }

        #region Barlox Animation

        private BarloxAnimation animation;

        private void InitializeAnimation()
        {
            // Load Barlox Animation
            StreamResourceInfo animData =
                Application.GetResourceStream(new Uri("Assets/chibi-small.ibxa", UriKind.Relative));
            animation = new BarloxAnimation(animData.Stream);
            animation.FrameChanged += (@s, e) => { AnimationImage.Source = e.NewFrame.Source; };
            animation.IsEnabled = true;

            // Attach interactivity
            AnimationImage.Tap += (@s, e) => animation.TriggerEvent("poke");
            AnimationImage.DoubleTap += (@s, e) => animation.TriggerEvent("knock");
        }

        #endregion
    }
}