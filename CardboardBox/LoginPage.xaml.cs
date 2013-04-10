// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// CardboardBox/LoginPage.xaml.cs
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
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using libDanbooru2;
using libWyvernzora.BarlogX.Animation;

namespace CardboardBox
{
    public partial class MainPage : PhoneApplicationPage
    {
        private readonly BarloxAnimation animation;

        // Constructor
        public MainPage()
        {


            InitializeComponent();

            // Load Appropriate Banner
            String bannerUrl = App.IsInDarkTheme() ? "/Assets/banner-dark.png" : "/Assets/banner-light.png";
            BitmapImage bannerSrc = new BitmapImage(new Uri(bannerUrl, UriKind.Relative));
            AppBanner.Source = bannerSrc;


            // Load Barlox Animation
            StreamResourceInfo animData =
                Application.GetResourceStream(new Uri("Assets/chibi-small.ibxa", UriKind.Relative));
            animation = new BarloxAnimation(animData.Stream);
            animation.FrameChanged += (@s, e) => { ImgChibi.Source = e.NewFrame.Source; };
            animation.IsEnabled = true;

            // Attach Event Handlers
            AttachEventHandlers();

            // Adjust UI State according to the session
            if (Session.Instance.Credentials == null)
                VisualStateManager.GoToState(this, "Default", false);
            else
                VisualStateManager.GoToState(this, "LoadingData", false);


            // Operations after page load
            Loaded += (@s, e) =>
                {
                    // Clear Back Stack
                    while (NavigationService.BackStack.Any())
                        NavigationService.RemoveBackEntry();

                    // If credentials already loaded skip login screen
                    if (Session.Instance.Credentials != null)
                    {
                        TextboxUsername.Text = Session.Instance.Credentials.Username;
                        VerifyCredentials();
                    }
                };
        }

        private void AttachEventHandlers()
        {
            ImgChibi.Tap += (@s, e) => animation.TriggerEvent("poke");

            // No Account? Sign Up!
            ButtonSignup.Click += (@s, e) =>
                {
                    WebBrowserTask task = new WebBrowserTask {Uri = new Uri(Session.SiteUrl + "/users/new")};
                    task.Show();
                };
            // Login!!
            ButtonLogin.Click += (@s, e) =>
                {
                    // Validate Data
                    if (String.IsNullOrWhiteSpace(TextboxUsername.Text))
                    {
                        TextBlockError.Text = "Username cannot be empty!";
                        VisualStateManager.GoToState(this, "Error", true);
                        TextboxUsername.Focus();
                        return;
                    }

                    if (TextboxPassword.Password.Length == 0)
                    {
                        TextBlockError.Text = "Password cannot be empty!";
                        VisualStateManager.GoToState(this, "Error", true);
                        TextboxPassword.Focus();
                        return;
                    }

                    // Verify Credentials
                    Session.Instance.Credentials = new DanbooruCredentials(TextboxUsername.Text,
                                                                           TextboxPassword.Password);
                    VerifyCredentials();
                };
        }

        private void VerifyCredentials()
        {
            VisualStateManager.GoToState(this, "LoadingData", true);

            DanbooruRequest<Post[]> request = new DanbooruRequest<Post[]>(Session.Instance.Credentials,
                                                                          Session.SiteUrl + Session.PostIndexUrl);
            request.AddArgument("limit", 1);

            request.ResponseReceived += (@s0, e0) => Dispatcher.BeginInvoke(() =>
                {
                    if (request.Status == (int) HttpStatusCode.OK)
                    {
                        // Everything good, go to next page

                        // TODO download data
                        Session.Instance.InitializeAsync(
                            () =>
                                {
                                    Session.Instance.ConfirmUserLogin();
                                    NavigationService.Navigate(new Uri("/HomePage.xaml", UriKind.Relative));
                                }
                            );

                        //NavigationService.Navigate(new Uri("/HomePage.xaml", UriKind.Relative));
                    }
                    else if (request.Status == (int) HttpStatusCode.Forbidden)
                    {
                        // Wrong password
                        TextBlockError.Text = "Incorrect username/password!";
                        VisualStateManager.GoToState(this, "Error", true);
                        TextboxPassword.Password = String.Empty;
                        TextboxPassword.Focus();
                    }
                    else
                    {
                        TextBlockError.Text = "Unexpected error, please try again.";
                        VisualStateManager.GoToState(this, "Error", true);
                    }
                });

            request.ExecuteRequest();
        }
    }
}