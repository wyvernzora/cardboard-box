using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using libWyvernzora.BarlogX.Animation;

namespace CardboardBox
{
    public partial class ViewPost : PhoneApplicationPage
    {
        private const Int32 PanMargin = 100;
        private BarloxAnimation animation;

        public ViewPost()
        {
            InitializeComponent();

            // Load Barlox Animation
            StreamResourceInfo animData =
                Application.GetResourceStream(new Uri("Assets/chibi-small.ibxa", UriKind.Relative));
            animation = new BarloxAnimation(animData.Stream);
            animation.FrameChanged += (@s, e) =>
                {
                    LoadingAnimationImage.Source = e.NewFrame.Source;
                };
            animation.IsEnabled = true;

            // Get Browser Size
            Int32 browserHeight = (Int32) PostBrowser.ActualHeight;
            Int32 browserWidth = (Int32) PostBrowser.ActualWidth;

            // Load Image
            String page = Session.Instance.PostViewerTemplate.GeneratePage(App.IsInDarkTheme() ? "000000" : "FFFFFF",
                                                                           Session.Instance.Selected);
            PostBrowser.NavigateToString(page);
            PostBrowser.LoadCompleted += (@s, e) =>
                {
                    VisualStateManager.GoToState(this, "Loaded", true);
                    animation.IsEnabled = false;
                };
            PostBrowser.NavigationFailed += (@s, e) =>
                {
                    System.Diagnostics.Debugger.Break();
                };
           // PostBrowser.ScriptNotify
        }
    }
}