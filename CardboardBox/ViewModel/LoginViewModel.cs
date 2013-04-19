using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Input;
using CardboardBox.API;
using CardboardBox.Model;
using CardboardBox.Utilities;
using Microsoft.Phone.Tasks;
using libWyvernzora.Patterns.MVVM;

namespace CardboardBox.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private const String LoadingState = "LoadingState";
        private const String ErrorState = "ErrorState";

        public LoginViewModel(LoginView view)
            : base(view.Dispatcher)
        {
            
        }

        #region Data Fields

        private String username;
        private String password;

        public String Username
        {
            get { return username; }
            set
            {
                if (username != value)
                {
                    username = value;
                    OnPropertyChanged("Username");
                }
            }
        }

        public String Password
        {
            get { return password; }
            set
            {
                if (password != value)
                {
                    password = value;
                    OnPropertyChanged("Password");
                }
            }
        }

        #endregion

        #region Commands

        private ICommand startupCommand;
        public ICommand StartupCommand
        {
            get
            {
                return startupCommand ?? (startupCommand = new ActionCommand(() =>
                    {
                        if (!NetworkInterface.GetIsNetworkAvailable())
                        {
                            Error = "No Internet Connection!";
                            OnChangeState(ErrorState);
                        }
                    }));
            }
        }

        private ICommand loginCommand;
        public ICommand LoginCommand
        {
            get { return loginCommand ?? (loginCommand = new ActionCommand(Login)); }
        }

        private ICommand signupCommand;
        public ICommand SignupCommand
        {
            get { return signupCommand ?? (signupCommand = new ActionCommand(Signup)); }
        }

        #endregion

        #region Methods

        private void Login()
        {
            // Check Network Connection
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                Error = "No Internet Connection!";
                OnChangeState(ErrorState);
                return;
            }

            // Goto Loading State
            OnChangeState(LoadingState);

            // Set Up Credentials
            if (Session.Instance.Credentials == null)
                Session.Instance.Credentials = new DanbooruCredentials(Username, Password);

            // Set Up Authentication
            var request = new DanbooruRequest<Post[]>(Session.Instance.Credentials,
                                                      Constants.SiteUrl + Danbooru2Client.PostIndex);
            request.AddArgument("limit", 1);

            request.ResponseReceived += (@s, e) => dispatcher.BeginInvoke(() =>
                {
                    switch (request.Status)
                    {
                        case 200:
                            Session.Instance.InitializeAsync(() =>
                                {
                                    Session.Instance.ConfirmUserLogin();
                                    NavigationHelper.Navigate(new Uri(Constants.HomeView, UriKind.Relative));
                                });
                            break;
                        case 403:
                            Error = "Incorrect Username/Password!";
                            OnChangeState(ErrorState);
                            break;
                        case 16:
                            Error = "Server is not responding!";
                            OnChangeState(ErrorState);
                            break;
                        default:
                            Error = String.Format("Error {0}, please try again later.", request.Status);
                            OnChangeState(ErrorState);
                            break;
                    }
                });

            request.ExecuteRequest();
        }

        private void Signup()
        {
            dispatcher.BeginInvoke(() =>
                {
                    WebBrowserTask task = new WebBrowserTask {Uri = new Uri(Constants.SiteUrl + "/users/new")};
                    task.Show();
                });
        }

        #endregion
    }
}
