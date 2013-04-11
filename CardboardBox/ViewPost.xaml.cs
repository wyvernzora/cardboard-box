﻿using System;
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

        public ViewPost()
        {
            InitializeComponent();

            // Load Barlox Animation
            StreamResourceInfo animData =
                Application.GetResourceStream(new Uri("Assets/chibi-small.ibxa", UriKind.Relative));
            BarloxAnimation animation = new BarloxAnimation(animData.Stream);
            animation.FrameChanged += (@s, e) =>
                {
                    LoadingAnimationImage.Source = e.NewFrame.Source;
                };
            animation.IsEnabled = true;

            // Get Browser Size
            Int32 browserHeight = (Int32) PostBrowser.ActualHeight;
            Int32 browserWidth = (Int32) PostBrowser.ActualWidth;

            // Load Image
            PostBrowser.NavigateToString(
                "<!DOCTYPE html><html style=\"overflow:hidden\"><body bgcolor=\"#000000\" style=\"width:100%; height=100%\"><meta name=\"viewport\" content=\"width=320, height=360, initial-scale=1\"/>" +
                "<div style=\"vertical-align:middle\"><center><img  style=\"width:100%; height=100%\" src=\"http://danbooru.donmai.us/data/9c348a0a483b4255a69101a53a5c9f5c.jpg\"/></center></div></body></html>"
                );
            //PostBrowser.Navigate(new Uri("http://danbooru.donmai.us/data/c7537035324b1902b317a5e395e99262.png"));
            PostBrowser.LoadCompleted += (@s, e) =>
                {
                    VisualStateManager.GoToState(this, "Loaded", true);
                };
        }
    }
}