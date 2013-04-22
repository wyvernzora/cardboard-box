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
        public const String LogoutState = "LogoutState";
        public const String FavChangedState = "FavChangedState";

        public HomeViewModel(HomeView view)
            : base(view.Dispatcher)
        {
            PostTuple.SetViewCommand(ViewCommand);
            favorites = new PostTupleCollection();
        }

        #region Data Fields

        #region New Posts

        public PostTupleCollection NewPosts
        { get { return Session.Instance.NewPosts; } }

        #endregion

        #region Favorites

        private PostTupleCollection favorites;

        public PostTupleCollection Favorites
        { get { return favorites; } }

        #endregion

        #region User Profile

        public User User
        { get { return Session.Instance.User; } }

        public UserLevel UserLevel
        {get { return Session.Instance.Level; }}

        #endregion
        
        #endregion

        #region Commands

        private ICommand pageLoadCommand;
        public ICommand PageLoadedCommand
        {
            get
            {
                return (pageLoadCommand ?? (pageLoadCommand = new ActionCommand(() =>
                    {
                        if (Session.Instance.ReloadFavorites)
                        {
                            lastAddTime = Int64.MaxValue;
                            favAtEnd = false;
                            favorites.Clear();
                            LoadMoreFavPosts();
                            Session.Instance.ReloadFavorites = false;
                        }
                    })));
            }
        }

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
                        //Session.Instance.Selected = p;
                        NavigationHelper.Navigate(new Uri(Constants.PostView, UriKind.Relative), p);
                    }));
            }
        }

        private ICommand logoutCommand;
        public ICommand LogoutCommand
        {
            get { return logoutCommand ?? (logoutCommand = new ActionCommand(() =>
                OnChangeState(LogoutState))); }
        }

        private ICommand settingsCommand;
        public ICommand SettingsCommand
        {
            get
            {
                return settingsCommand ?? (settingsCommand = new ActionCommand(() => NavigationHelper.Navigate(new Uri(Constants.SettingsVIew, UriKind.Relative))));
            }
        }

        private ICommand loadFavPostsCommand;
        public ICommand LoadFavoritePostsCommand
        {
            get { return loadFavPostsCommand ?? (loadFavPostsCommand = new ActionCommand(LoadMoreFavPosts)); }
        }

        #endregion

        #region Methods and Related Stuff

        #region New Posts

        private Boolean newPostsLoading;
        private Boolean newPostsAtEnd;

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
                    Int32 page = (Session.Instance.NewPosts.Count * 3) / Session.Instance.Client.PageSize + 1;
                    Post[] posts = Session.Instance.Client.GetPosts(page, 1, Session.Instance.ResolveQueryString(""));

                    if (posts.Length < Session.Instance.Client.PageSize)
                        newPostsAtEnd = true;

                    dispatcher.BeginInvoke(() =>
                        {
                            NewPosts.AddRange(posts);

                            newPostsLoading = false;
                            Logging.D("HomeViewModel.LoadMoreNewPosts(): Loading complete!");
                        });
                });
        }

        #endregion

        #region Favorites

        private Int64 lastAddTime;
        private Boolean favLoading;
        private Boolean favAtEnd;

        private void LoadMoreFavPosts()
        {
            if (favLoading)
                return;
            if (favAtEnd)
                return;

            favLoading = true;

            Logging.D("HomeViewModel.LoadMoreFavPosts(): Loading more posts...");

            ThreadPool.QueueUserWorkItem(callback =>
                {
                    Post[] posts = Database.Instance.GetFavorites(Session.Instance.User, lastAddTime);

                    if (posts.Length < Database.PostPageSize)
                        favAtEnd = true;
                    else
                        lastAddTime = posts[posts.Length - 1].LastAddTime;

                    dispatcher.BeginInvoke(() =>
                        {
                            favorites.AddRange(posts);
                            favLoading = false;

                            Logging.D("HomeViewModel.LoadMoreFavPosts(): More posts loaded!");
                            OnChangeState(FavChangedState);
                        });
                });

        }

        #endregion

        #endregion

    }
}
