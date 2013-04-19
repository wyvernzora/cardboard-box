using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Input;
using CardboardBox.API;
using CardboardBox.Model;
using CardboardBox.UI;
using CardboardBox.Utilities;
using libWyvernzora.Patterns.MVVM;
using libWyvernzora.Core;

namespace CardboardBox.ViewModel
{
    public class HomeViewModel : ViewModelBase
    {
        public HomeViewModel(HomeView view)
            : base(view.Dispatcher)
        {
            PostTuple.SetViewCommand(ViewCommand);
        }

        #region Data Fields

        #region New Posts

        public ObservableCollection<PostTuple> NewPosts
        { get { return Session.Instance.NewPosts; } }

        #endregion

        #region User Profile

        public User User
        { get { return Session.Instance.User; } }

        public UserLevel UserLevel
        {get { return Session.Instance.Level; }}

        #endregion


        #endregion

        #region Commands

        private ICommand loadNewPostsCommand;
        public ICommand LoadNewPostsCommand
        { get { return loadNewPostsCommand ?? (loadNewPostsCommand = new ActionCommand(LoadMoreNewPosts)); } }

        private ICommand searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                return searchCommand ??
                       (searchCommand =
                        new ActionCommand(
                            () => NavigationHelper.Navigate(new Uri(Constants.SearchView, UriKind.Relative))));
            }
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

        #region Methods and Related Stuff

        #region New Posts

        private Boolean newPostsLoading = false;
        private Boolean newPostsAtEnd = false;

        private void LoadMoreNewPosts()
        {
            if (newPostsLoading)
                return;
            if (newPostsAtEnd)
                return;

            newPostsLoading = true;

            Logging.D("HomeViewModel.LoadMoreNewPosts(): Loading more posts...");

            ThreadPool.QueueUserWorkItem(callback =>
                {
                    PostTuple[] tuples = Session.Instance.GetMoreNewPosts(1);

                    if (tuples.Length < 20)
                        newPostsAtEnd = true;

                    dispatcher.BeginInvoke(() =>
                        {
                            tuples.ForEach(t => NewPosts.Add(t));
                            newPostsLoading = false;
                            Logging.D("HomeViewModel.LoadMoreNewPosts(): Loading complete!");
                        });
                });
        }

        #endregion

        #endregion

    }
}
