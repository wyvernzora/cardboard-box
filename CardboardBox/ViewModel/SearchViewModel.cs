using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using CardboardBox.API;
using CardboardBox.Model;
using CardboardBox.UI;
using CardboardBox.Utilities;
using libWyvernzora.Patterns.MVVM;
using libWyvernzora.Core;
using System.Net;

namespace CardboardBox.ViewModel
{
    public class SearchViewModel : ViewModelBase
    {
        // Actual States
        public const String LoadingState = "LoadingState";
        public const String LoadedState = "LoadedState";

        // Phony States
        public const String LogoutState = "LogoutState";
        public const String NoResultState = "NoResultState";

        public SearchViewModel(SearchView view) 
            : base(view.Dispatcher)
        {
            //SearchResults = new ObservableCollection<PostTuple>();
            SearchResults = new PostTupleCollection();
        }

        #region Data Field

        private String searchString ;
        private String[] query;
        
        public PostTupleCollection SearchResults { get; private set; }

        public String SearchString
        {
            get { return searchString; }
            set
            {
                if (searchString != value)
                {
                    searchString = value;
                    OnPropertyChanged("SearchString");
                }
            }
        }

        #endregion

        #region Commands

        private ICommand searchCommand;
        public ICommand SearchCommand
        {
            get { return searchCommand ?? (searchCommand = new ActionCommand(Search)); }
        }

        private ICommand loadPageCommand;
        public ICommand LoadPageCommand
        {
            get { return loadPageCommand ?? (loadPageCommand = new ActionCommand(() => LoadPage(true))); }
        }

        private ICommand viewCommand;
        public ICommand ViewCommand
        {
            get
            {
                return viewCommand ?? (viewCommand = new ParamActionCommand<Post>(p => NavigationHelper.Navigate(new Uri(Constants.PostView, UriKind.Relative), p)));
            }
        }

        private ICommand bookmarkCommand;
        public ICommand BookmarkCommand
        { get { return bookmarkCommand ?? (bookmarkCommand = new ActionCommand(() => { })); } }


        private ICommand logoutCommand;
        public ICommand LogoutCommand
        {
            get
            {
                return logoutCommand ?? (logoutCommand = new ActionCommand(() =>
                    OnChangeState(LogoutState)));
            }
        }

        private ICommand settingsCommand;
        public ICommand SettingsCommand
        {
            get
            {
                return settingsCommand ?? (settingsCommand = new ActionCommand(() => NavigationHelper.Navigate(new Uri(Constants.SettingsVIew, UriKind.Relative))));
            }
        }


        #endregion

        #region Methods

        private Int32 resultPage;
        private Boolean resultsLoading;
        private Boolean resultsAtEnd;
        
        private void Search()
        {
            // Check User Level and Limits
            Int32 levelLimit = Session.Instance.Level.TagLimit;

            query = Session.Instance.ResolveQueryString(searchString);
            if (query.Length > levelLimit)
            {
                MessageBox.Show(String.Format("Your user level of {0} only allows you to search {1} tags at once!", 
                    Session.Instance.Level.Name, levelLimit), "Hey hey! Slow down!", MessageBoxButton.OK);
                return;
            }

            // Hook up to stats server
            ThreadPool.QueueUserWorkItem(callback =>
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.CreateHttp(@"http://devfish.org/opix/query.php");
                    request.Method = "POST";

                    Byte[] data = System.Text.Encoding.UTF8.GetBytes("query=" + HttpUtility.UrlEncode(searchString));

                    request.BeginGetRequestStream(c =>
                        {
                            // Get Response
                            if (c == null) throw new Exception();

                            try
                            {
                                libWyvernzora.IO.StreamEx stream = new libWyvernzora.IO.StreamEx(request.EndGetRequestStream(c));
                                stream.WriteBytes(data);
                                stream.Close();
                            }
                            catch
                            {
                                // suppress
                            }
                        }, request);

                    request.BeginGetResponse(c => { /* suppress */ }, null);
                });

            // Clear Current Results
            SearchResults.Clear();
            resultPage = 1;
            resultsLoading = false;
            resultsAtEnd = false;

            // Go For Next Page
            LoadPage(false);
        }

        private void LoadPage(Boolean silent)
        {
            if (query == null)
                return;
            if (resultsLoading)
                return;
            if (resultsAtEnd)
                return;

            resultsLoading = true;
            if (!silent)
                OnChangeState(LoadingState);

            ThreadPool.QueueUserWorkItem(callback =>
                {
                    Session.Instance.AssertUserLevelLimit(query);
                    Post[] posts = Session.Instance.Client.GetPosts(resultPage++, 1, query);

                    if (posts.Length < Session.Instance.Client.PageSize)
                        resultsAtEnd = true;

                    dispatcher.BeginInvoke(() =>
                        {
                            SearchResults.AddRange(posts);

                            // Show "no result" if server returned empty array; go to loaded state otherwise
                            OnChangeState(SearchResults.Count == 0 ? NoResultState : LoadedState);

                            resultsLoading = false;
                        });
                });
        }

        #endregion
    }
}
