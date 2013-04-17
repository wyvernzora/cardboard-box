using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
using CardboardBox.API;
using CardboardBox.UI;
using Microsoft.Phone.Controls;
using libDanbooru2;
using libWyvernzora.BarlogX.Animation;

namespace CardboardBox
{
    public partial class ViewPost : PhoneApplicationPage
    {
        #region Constants

        private const Int32 PostPageLoadingThreshold = 180;

        private const Int32 TagFontSize = 26;

        private readonly String[] SizeUnits =
            {
                "B", "KB", "MB", "GB"
            };

        #endregion

        private BarloxAnimation animation;

        private ObservableCollection<Comment> comments;
        private ObservableCollection<PostTuple> relatedPosts;
        private ScrollViewMonitor monitor;
        private Int32 relatedPages = 1;
        private Boolean relatedLoading = false;
        private Boolean reachedEnd = false;

        private Post p;

        public ViewPost()
        {
            p = Session.Instance.Selected;

            InitializeComponent();

            ApplicationBar.IsVisible = false;

            // Initialize fields
            comments = new ObservableCollection<Comment>();
            relatedPosts = new ObservableCollection<PostTuple>();
            RelatedList.ItemsSource = relatedPosts;

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
            AttachEventHandlers();

            // Attach Post Bindings
            CommentsList.ItemsSource = comments;
            CommentsList.DisplayMemberPath = "Body";
        }

        private void PageLoaded()
        {
            Logging.D("ViewPost: Loaded.");

            // DO NOT LOAD F**KING GIFS!!! THEY BLOW UP YOUR PHONE!!!
            if (p.FileExtension.ToLower() == "gif")
            {
                Logging.D("ViewPost: GIF post rejected.");
                MessageBox.Show("Loading this post is a bad idea because it will use way too much memory. We apologize for inconvenience.", "We've got some problems", MessageBoxButton.OK);
                NavigationService.GoBack();
            }

            // Lock Down the Pivot
            PivotRoot.IsLocked = true;

            LoadPost(p);

            // Get Related Data that requires async handling
            ThreadPool.QueueUserWorkItem(@a =>
                {
                    LoadComments();

                    relatedLoading = true;

                    PostTuple[] tuples;
                    if (p.HasChildren || p.ParentID.HasValue)
                    {
                        Int32 parentId = p.HasChildren ? p.ID : p.ParentID.Value;
                        tuples = Session.Instance.ExecutePostQuery(relatedPages++, 1,
                                                                               "parent:" +
                                                                               parentId.ToString(
                                                                                   CultureInfo.InvariantCulture));
                        if (tuples.Length < Session.PostRequestPageSize) reachedEnd = true;
                    } else tuples = new PostTuple[0];

                    Dispatcher.BeginInvoke(() =>
                        {
                            if (tuples.Length != 0)
                            {
                                // Add related images
                                foreach (var v in tuples)
                                    relatedPosts.Add(v);
                            }
                            else
                                RelatedPivotItem.Visibility = Visibility.Collapsed;
                            relatedLoading = false;

                            // Load Image from template
                            String page =
                                Session.Instance.PostViewerTemplate.GeneratePage(
                                    App.IsInDarkTheme() ? "000000" : "FFFFFF",
                                    p);
                            PostBrowser.NavigateToString(page);
                        });
                });
        }

        private void AttachEventHandlers()
        {
            Loaded += (@s, e) => PageLoaded();
            RelatedList.Loaded += (@o, a) =>
            {
                monitor = new ScrollViewMonitor(RelatedList);
                monitor.Scroll += (@s, e) =>
                {
                    if (e.OffsetY > e.MaxY - PostPageLoadingThreshold)
                        LoadRelated();
                };
            };
            RelatedList.Tap += (@s, e) =>
            {
                var item = RelatedList.SelectedItem as PostTuple;
                if (item == null) return;

                Int32 row = (Int32)Math.Floor(e.GetPosition(RelatedList).X / 130);

                switch (row)
                {
                    case 0:
                        Session.Instance.Selected = item.First;
                        break;
                    case 1:
                        Session.Instance.Selected = item.Second;
                        break;
                    case 2:
                        Session.Instance.Selected = item.Third;
                        break;
                    default:
                        return;
                }

                if (Session.Instance.Selected != null)
                    NavigateToRelated(Session.Instance.Selected);
            };
            PivotRoot.Loaded += (@s, e) =>
            {
                if (!p.HasChildren && !p.ParentID.HasValue)
                    PivotRoot.Items.Remove(RelatedPivotItem);
            };
            PivotRoot.SelectionChanged += (@s, e) =>
                {
                    ApplicationBar.IsVisible = PivotRoot.SelectedIndex == 0;
                };
            PostBrowser.LoadCompleted += (@s, e) =>
            {
                VisualStateManager.GoToState(this, "DataLoaded", true);
                animation.IsEnabled = false;
                PivotRoot.IsLocked = false;
                ApplicationBar.IsVisible = true;
            };
            PostBrowser.NavigationFailed += (@s, e) =>
            {
                // Should never fire.
                Logging.D("");
                System.Diagnostics.Debugger.Break();
            };
            PostBrowser.ScriptNotify += (@s, e) =>
            {
                MessageBox.Show("It seems like this image cannot be loaded at this time. Maybe you should try again later.", "We've got some problems", MessageBoxButton.OK);
                NavigationService.GoBack();
            };
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
            request.AddArgument("search[post_id]", p.ID);
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

        /// <summary>
        /// Loads related posts BLOCKING the CALLER THREAD
        /// </summary>
        private void LoadRelated()
        {
            if (!p.HasChildren && !p.ParentID.HasValue)
                return; // No related posts, skip
            if (relatedLoading || reachedEnd) return;
            Int32 parentId = p.HasChildren ? p.ID : p.ParentID.Value;

            relatedLoading = true;

            ThreadPool.QueueUserWorkItem(@a =>
                {
                    PostTuple[] tuples = Session.Instance.ExecutePostQuery(relatedPages++, 1,
                                                                           "parent:" +
                                                                           parentId.ToString(
                                                                               CultureInfo.InvariantCulture));
                    if (tuples.Length < Session.PostRequestTupleSize) reachedEnd = true;

                    Dispatcher.BeginInvoke(() =>
                        {
                            foreach (var v in tuples)
                                relatedPosts.Add(v);

                            relatedLoading = false;
                        });
                });
        }


        private void NavigateToRelated(Post post)
        {
            // Kick in animation
            VisualStateManager.GoToState(this, "DataLoading", true);

            LoadPost(post);

            // Get Related Data that requires async handling
            ThreadPool.QueueUserWorkItem(@a =>
            {
                LoadComments();

               Dispatcher.BeginInvoke(() =>
                {
                    // Load Image from template
                    String page =
                        Session.Instance.PostViewerTemplate.GeneratePage(
                            App.IsInDarkTheme() ? "000000" : "FFFFFF", p);
                    PostBrowser.NavigateToString(page);
                });
            });

        }

        /// <summary>
        /// Loads the post on a background thread,
        /// but does not change the UI state.
        /// </summary>
        /// <param name="post"></param>
        private void LoadPost(Post post)
        {
            // Reset all parameters
            PivotRoot.SelectedIndex = 0;
            animation.IsEnabled = true;
            PivotRoot.IsLocked = true;
            comments = new ObservableCollection<Comment>();

            // Switch Current Post
            p = post;
            PivotRoot.Title = "POST #" + p.ID.ToString(CultureInfo.InvariantCulture);

            #region loadTags Lambda

            Action<String[], TextBlock, WrapPanel, Color> loadTags = (@tags, header, panel, color) =>
                {
                    panel.Children.Clear();

                    if (tags.Length == 0)
                    {
                        header.Visibility = Visibility.Collapsed;
                        panel.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        SolidColorBrush brush = new SolidColorBrush(color);
                        foreach (var t in tags)
                        {
                            TextBlock block = new TextBlock { FontSize = TagFontSize, Tag = t };
                            block.Tap += (@s, e) =>
                            {
                                String tag = ((TextBlock)s).Tag as string;
                                if (tag == null) return;

                                Session.Instance.Navigate(new Uri("/Modules/SearchPage.xaml?query=" + tag, UriKind.Relative));
                            };

                            block.Inlines.Add(new Run
                            {
                                Text = t,
                                Foreground = brush
                            });

                            block.Margin = new Thickness(4, 2, 4, 2);
                            panel.Children.Add(block);
                        }

                        header.Visibility = Visibility.Visible;
                        panel.Visibility = Visibility.Visible;
                    }
                };

            #endregion
            
            // Load up tags
            String[] cpTags = p.CopyrightTagsString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            loadTags(cpTags, CopyrightTagHeader, CopyrightTagPanel, Color.FromArgb(0xFF, 0xEE, 0x66, 0xEE));
            String[] chTags = p.CharacterTagsString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            loadTags(chTags, CharacterTagHeader, CharacterTagPanel, Color.FromArgb(0xFF, 0x44, 0xEE, 0x44));
            String[] arTags = p.ArtistTagsString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            loadTags(arTags, ArtistTagHeader, ArtistTagPanel, Color.FromArgb(0xFF, 0xEE, 0x66, 0x66));
            String[] gTags = p.GeneralTagsString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            loadTags(gTags, GeneralTagHeader, GeneralTagPanel, Color.FromArgb(0xFF, 0x44, 0x77, 0xFF));

            // Load Post Info
            PostIdTextBlock.Text = p.ID.ToString(CultureInfo.InvariantCulture);
            UploaderTextBlock.Text = p.Author;

            // Convert Byte size to something more readable
            Double size = p.FileSize;
            Int32 order = 0;
            while (size >= 1024 && order + 1 < SizeUnits.Length)
            {
                order++;
                size = size / 1024.0;
            }
            SizeTextBlock.Text = String.Format("{0:0.#} {1} ({2}x{3})", size, SizeUnits[order], p.Width, p.Height);
            RatingTextBlock.Text = p.Rating.ToString();
        }
        
        #endregion
    }
}