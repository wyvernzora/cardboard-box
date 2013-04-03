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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using libDanbooru2;

namespace CardboardBox
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Load Appropriate Banner
            String bannerUrl = App.IsInDarkTheme() ? "/Assets/banner-dark.png" : "/Assets/banner-light.png";
            BitmapImage bannerSrc = new BitmapImage(new Uri(bannerUrl, UriKind.Relative));
            AppBanner.Source = bannerSrc;

            // Attach Event Handlers
            AttachEventHandlers();

            // Operations after page load
            Loaded += (@s, e) =>
                {
                    VisualStateManager.GoToState(this, "Default", false);
                };

        }

        private void AttachEventHandlers()
        {
            // No Account? Sign Up!
            ButtonSignup.Click += (@s, e) =>
                {
                    var task = new WebBrowserTask {Uri = new Uri(Constants.DanbooruSignUpUrl)};
                    task.Show();
                };
            // Login!!
            ButtonLogin.Click += (@s, e) =>
                {
                    VisualStateManager.GoToState(this, "LoggingIn", true);

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
                    App.Credentials = new DanbooruCredentials(TextboxUsername.Text, TextboxPassword.Password);
                    VerifyCredentials();
                };
        }
    
        private void VerifyCredentials()
        {
            DanbooruRequest<Post[]> request = new DanbooruRequest<Post[]>(App.Credentials, Constants.DanbooruPostIndexUrl);
            request.AddArgument("limit", 1);

            request.ResponseReceived += (@s0, e0) => Dispatcher.BeginInvoke(() =>
            {
                if (request.Status == (int)HttpStatusCode.OK)
                {
                    // Everything good, go to next page
                    VisualStateManager.GoToState(this, "Default", true);
                    NavigationService.Navigate(new Uri("/HomePage.xaml", UriKind.Relative));
                }
                else if (request.Status == (int)HttpStatusCode.Forbidden)
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