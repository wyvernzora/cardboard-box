using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using libDanbooru2;
using libWyvernzora.BarlogX.Animation;

namespace CardboardBox
{
    public partial class ViewPost : PhoneApplicationPage
    {
        private const Int32 PanMargin = 100;
        private BarloxAnimation animation;

        private ObservableCollection<Comment> comments;

        public ViewPost()
        {
            InitializeComponent();

            // Initialize fields
            comments = new ObservableCollection<Comment>();


            // Load Barlox Animation
            StreamResourceInfo animData =
                Application.GetResourceStream(new Uri("Assets/chibi-small.ibxa", UriKind.Relative));
            animation = new BarloxAnimation(animData.Stream);
            animation.FrameChanged += (@s, e) =>
                {
                    LoadingAnimationImage.Source = e.NewFrame.Source;
                };
            animation.IsEnabled = true;

            // Attach Loaded Event Handler
            base.Loaded += (@s, e) => PageLoaded();

            // Attach Post Bindings
            CommentsList.ItemsSource = comments;
            CommentsList.DisplayMemberPath = "Body";

            // Hook up post browser events
            PostBrowser.LoadCompleted += (@s, e) =>
                {
                    VisualStateManager.GoToState(this, "Loaded", true);
                    animation.IsEnabled = false;
                    PivotRoot.IsLocked = false;
                };
            PostBrowser.NavigationFailed += (@s, e) =>
                {
                    // Should never fire.
                    System.Diagnostics.Debugger.Break();
                };
            PostBrowser.ScriptNotify += (@s, e) =>
                {
                    MessageBox.Show("It seems like this image cannot be loaded at this time. Maybe you should try again later.", "We've got some problems", MessageBoxButton.OK);
                    NavigationService.GoBack();
                };
        }

        private void PageLoaded()
        {
            // Lock Down the Pivot
            PivotRoot.IsLocked = true;

            // Get Related Data
            ThreadPool.QueueUserWorkItem(@a =>
                {
                    LoadComments();

                    Dispatcher.BeginInvoke(() =>
                        {
                            // Load Image from template
                            String page =
                                Session.Instance.PostViewerTemplate.GeneratePage(
                                    App.IsInDarkTheme() ? "000000" : "FFFFFF",
                                    Session.Instance.Selected);
                            PostBrowser.NavigateToString(page);
                        });
                });

            
        }


        #region Update Methods

        /// <summary>
        /// Loads comments blocking the CALLER THREAD
        /// </summary>
        private void LoadComments()
        {
            // Load Comments
            var request = new DanbooruRequest<Comment[]>(Session.Instance.Credentials,
                                                         Session.SiteUrl + Session.CommentIndexUrl);
            request.AddArgument("search[post_id]", Session.Instance.Selected.ID);
            request.AddArgument("group_by", "comment"); // Wierd argument required by Danbooru 2.0 specification.

            request.ExecuteRequest(Session.Instance.Cookie);
            while (request.Status < 0) Thread.Sleep(20); // Wait for request to return

            if (request.Status != 200)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    comments.Clear();
                    CommentsList.Visibility = Visibility.Collapsed;
                    NoCommentTextBlock.Text = "Opix could not load comments because of an error.\nRefreshing might do the trick.";
                    NoCommentTextBlock.Visibility = Visibility.Visible;
                });
                return;
            }

            if (request.Result.Length == 0)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    comments.Clear();
                    CommentsList.Visibility = Visibility.Collapsed;
                    NoCommentTextBlock.Text = "It seems like there are no comments here.\nWanna be the first to post one?";
                    NoCommentTextBlock.Visibility = Visibility.Visible;
                });
                return;
            }
            
            
            Dispatcher.BeginInvoke(() =>
                {
                    comments.Clear();
                    foreach (var c in request.Result)
                        comments.Add(c);

                    CommentsList.Visibility = Visibility.Visible;
                    NoCommentTextBlock.Visibility = Visibility.Collapsed;
                });
        }

        #endregion
    }
}