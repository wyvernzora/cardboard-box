using System;
using System.Collections.Generic;
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
using CardboardBox.Barlox;
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
#if DEBUG
            App.Credentials = new DanbooruCredentials("CardboardBoxTest", "mikumiku");
#endif

            InitializeComponent();

            // Load Appropriate Banner
            String bannerUrl = App.IsInDarkTheme() ? "/Assets/banner-dark.png" : "/Assets/banner-light.png";
            BitmapImage bannerSrc = new BitmapImage(new Uri(bannerUrl, UriKind.Relative));
            AppBanner.Source = bannerSrc;

            // Load Barlox Animation
            StreamResourceInfo animData = Application.GetResourceStream(new Uri("Assets/chibi-small.bxa", UriKind.Relative));
            BarloxAnimation animation = new BarloxAnimation(animData.Stream);
            animation.FrameChanged += (@s, e) =>
                {
                    ImgChibi.Source = e.NewFrame.Source;
                };
            animation.IsEnabled = true;

            // Attach Event Handlers
            AttachEventHandlers();

            // Operations after page load
            Loaded += (@s, e) =>
                {
                    if (App.Credentials == null)
                        VisualStateManager.GoToState(this, "Default", false);
                    else
                    {
                        TextboxUsername.Text = App.Credentials.Username;
                        VerifyCredentials();
                    }
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
            
            VisualStateManager.GoToState(this, "LoadingData", true);

            DanbooruRequest<Post[]> request = new DanbooruRequest<Post[]>(App.Credentials, Constants.DanbooruPostIndexUrl);
            request.AddArgument("limit", 1);

            request.ResponseReceived += (@s0, e0) => Dispatcher.BeginInvoke(() =>
            {
                if (request.Status == (int)HttpStatusCode.OK)
                {
                    // Everything good, go to next page

                    // TODO download data
                    CardboardBoxSession.Instance.InitializeAsync(
                        () =>
                            {
                                NavigationService.Navigate(new Uri("/HomePage.xaml", UriKind.Relative));
                            }
                        );

                    //NavigationService.Navigate(new Uri("/HomePage.xaml", UriKind.Relative));
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