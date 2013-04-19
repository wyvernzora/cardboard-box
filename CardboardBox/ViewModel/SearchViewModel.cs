using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using CardboardBox.Model;
using CardboardBox.UI;
using CardboardBox.Utilities;
using libWyvernzora.Patterns.MVVM;
using libWyvernzora.Core;

namespace CardboardBox.ViewModel
{
    public class SearchViewModel : ViewModelBase
    {
        public const String LoadingState = "LoadingState";
        public const String LoadedState = "LoadedState";
        public const String NoResultState = "NoResultState";

        public SearchViewModel(SearchView view) 
            : base(view.Dispatcher)
        {
            SearchResults = new ObservableCollection<PostTuple>();
        }

        #region Data Field

        private String searchString ;
        private String[] query;
        
        public ObservableCollection<PostTuple> SearchResults { get; private set; }

        public String SearchString
        {
            get { return searchString; }
            set
            {
                if (searchString != value)
                {
                    searchString = value;
                    query = Session.Instance.ResolveQueryString(searchString);
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
                return viewCommand ?? (viewCommand = new ParamActionCommand<Post>(p =>
                {
                    Session.Instance.Selected = p;
                    NavigationHelper.Navigate(new Uri(Constants.PostView, UriKind.Relative));
                }));
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

            if (query.Length > levelLimit)
            {
                MessageBox.Show(String.Format("Your user level of {0} only allows you to search {1} tags at once!", 
                    Session.Instance.Level.Name, levelLimit), "Hey hey! Slow down!", MessageBoxButton.OK);
                return;
            }

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
                    PostTuple[] tuples = Session.Instance.ExecutePostQuery(resultPage++, 1, query);

                    if (tuples.Length < 20) // TODO Constant?
                        resultsAtEnd = true;

                    dispatcher.BeginInvoke(() =>
                        {
                            tuples.ForEach(t => SearchResults.Add(t));

                            // Show "no result" if server returned empty array; go to loaded state otherwise
                            OnChangeState(SearchResults.Count == 0 ? NoResultState : LoadedState);

                            resultsLoading = false;
                        });
                });
        }

        #endregion
    }
}
