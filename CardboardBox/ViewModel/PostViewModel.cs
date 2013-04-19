using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using CardboardBox.Model;
using CardboardBox.UI;
using CardboardBox.Utilities;
using libWyvernzora.Patterns.MVVM;
using libWyvernzora.Core;

namespace CardboardBox.ViewModel
{
    public class PostViewModel : ViewModelBase
    {
        // Actual States
        public const String LoadingState = "LoadingState1";
        public const String LoadedState = "LoadedState";

        // Phony States
        public const String BrowserNavigateState = "BrowserNavigate";
        public const String NoCommentState = "NoCommentState";


        public PostViewModel(PostView view) 
            : base(view.Dispatcher)
        {
            Comments = new ObservableCollection<Comment>();

        }

        #region Data Properties

        public Post Post { get; set; }

        public ObservableCollection<Comment> Comments { get; private set; }

        public String ViewTitle
        { get { return "POST #" + Post.ID; } }

        public String PostTemplate
        {
            get
            {
               return Session.Instance.PostViewerTemplate.GeneratePage(
                                       App.IsInDarkTheme() ? "000000" : "FFFFFF",
                                       Post);
            }
        }

        #endregion

        #region Commands

        private ICommand loadPostCommand;
        public ICommand LoadPostCommand
        {
            get { return loadPostCommand ?? (loadPostCommand = new ActionCommand(LoadPost)); }
        }

        private ICommand searchTagCommand;
        public ICommand SearchTagCommand
        {
            get
            {
                return searchTagCommand ?? (searchTagCommand = new ParamActionCommand<String>(t => NavigationHelper.Navigate(new Uri(Constants.SearchView, UriKind.Relative), t)));
            }
        }

        #endregion

        #region Methods

        private void LoadPost()
        {
            OnChangeState(LoadingState, false);
            ThreadPool.QueueUserWorkItem(callback =>
                {
                    // Parent
                    Comment[] comments = Session.Instance.Client.GetComments(Post.ID);
                
                    dispatcher.BeginInvoke(() =>
                        {
                            comments.ForEach(c => Comments.Add(c));

                            if (Comments.Count == 0)
                                OnChangeState(NoCommentState);

                            OnChangeState(BrowserNavigateState);
                        });
                });
        }

        #endregion
    }
}
