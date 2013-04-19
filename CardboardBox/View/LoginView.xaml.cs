// ReSharper disable CheckNamespace

// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// CardboardBox/LoginView.xaml.cs
// --------------------------------------------------------------------------------
// Copyright (c) 2013, Jieni Luchijinzhou a.k.a Aragorn Wyvernzora
// 
// This file is a part of CardboardBox.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
// of the Software, and to permit persons to whom the Software is furnished to do 
// so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using CardboardBox.ViewModel;
using libWyvernzora.BarlogX.Animation;


namespace CardboardBox
{
    public partial class LoginView
    {
        private readonly LoginViewModel viewModel;

        /// <summary>
        ///     Constructor.
        ///     Everyone know what a constructor does.
        /// </summary>
        public LoginView()
        {
            InitializeComponent();

            // Create ViewModel
            viewModel = new LoginViewModel(this);
            viewModel.ChangeState += (@s, e) => VisualStateManager.GoToState(this, e.State, e.Transition);
            DataContext = viewModel;

            // Load Appropriate Banner
            String bannerUrl = App.IsInDarkTheme()
                                   ? "/Assets/application-banner-dark.png"
                                   : "/Assets/application-banner-light.png";
            BitmapImage bannerSrc = new BitmapImage(new Uri(bannerUrl, UriKind.Relative));
            AppBanner.Source = bannerSrc;

            // Initialize Animation
            InitializeAnimation();

            // Operations after page load
            Loaded += (@s, e) => PageLoaded();
        }

        /// <summary>
        ///     Initialize items that require the page to be
        ///     completely loaded.
        /// </summary>
        private void PageLoaded()
        {
            // Clear Back Stack
            while (NavigationService.BackStack.Any())
                NavigationService.RemoveBackEntry();

            // If credentials already loaded skip login screen
            if (Session.Instance.Credentials != null)
            {
                viewModel.Username = Session.Instance.Credentials.Username;
                Focus();
                viewModel.LoginCommand.Execute(null);
            }
        }

        #region Barlox Animation

        private BarloxAnimation animation;

        private void InitializeAnimation()
        {
            // Load Barlox Animation
            StreamResourceInfo animData =
                Application.GetResourceStream(new Uri("Assets/chibi-small.ibxa", UriKind.Relative));
            animation = new BarloxAnimation(animData.Stream);
            animation.FrameChanged += (@s, e) => { ImgChibi.Source = e.NewFrame.Source; };
            animation.IsEnabled = true;

            // Attach interactivity
            ImgChibi.Tap += (@s, e) => animation.TriggerEvent("poke");
        }

        #endregion
    }
}