// ReSharper disable CheckNamespace

// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// CardboardBox/ViewPost.xaml.cs
// --------------------------------------------------------------------------------
// Copyright (c) 2013, Jieni Luchijinzhou a.k.a Aragorn Wyvernzora
// 
// This file is a part of CardboardBox.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
// of the Software, and to permit persons to whom the Software is furnished to do 
// so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Resources;
using CardboardBox.API;
using CardboardBox.Model;
using CardboardBox.UI;
using CardboardBox.ViewModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using libWyvernzora.BarlogX.Animation;

namespace CardboardBox
{
    public partial class PostView
    {
        #region Constants

        private const Int32 PostPageLoadingThreshold = 180;

        private const Int32 TagFontSize = 26;

        #endregion

        private readonly BarloxAnimation animation;
        private readonly PostViewModel viewModel;
        

        public PostView()
        {
            // Create ViewModel
            viewModel = new PostViewModel(this);
            DataContext = viewModel;
            viewModel.ChangeState += (@s, e) =>
            {
                if (e.State == PostViewModel.NoCommentState)
                {
                    NoCommentTextBlock.Visibility = Visibility.Visible;
                }
                if (e.State == PostViewModel.BrowserNavigateState)
                {
                    PostBrowser.NavigateToString(viewModel.PostTemplate);
                }
                VisualStateManager.GoToState(this, e.State, e.Transition);
            };
            viewModel.Post = NavigationHelper.NavigationArgument as Post;

            // Initialize UI
            InitializeComponent();
            ApplicationBar.IsVisible = false;
            
            // Load Barlox Animation
            StreamResourceInfo animData =
                Application.GetResourceStream(new Uri("Assets/chibi-small.ibxa", UriKind.Relative));
            animation = new BarloxAnimation(animData.Stream);
            animation.FrameChanged += (@s, e) => { LoadingAnimationImage.Source = e.NewFrame.Source; };
            animation.IsEnabled = true;

            // Attach Loaded Handler
            Loaded += (@s, e) => PageLoaded();
            PivotRoot.SelectionChanged += (@s, e) => { ApplicationBar.IsVisible = PivotRoot.SelectedIndex == 0; };

            // Attach Post Browser Handlers
            PostBrowser.LoadCompleted += (@s, e) =>
            {
                VisualStateManager.GoToState(this, "LoadedState", true);
                animation.IsEnabled = false;
                PivotRoot.IsLocked = false;
                ApplicationBar.IsVisible = true;
            };
            PostBrowser.NavigationFailed += (@s, e) =>
            {
                // Should never fire.
                Logging.D("ViewPost: Navigation Failed");
                Debugger.Break();
            };
            PostBrowser.ScriptNotify += (@s, e) =>
            {
                MessageBox.Show(
                    "It seems like this image cannot be loaded at this time. Maybe you should try again later.",
                    "We've got some problems", MessageBoxButton.OK);
                NavigationService.GoBack();
            };
        }

        private void PageLoaded()
        {
            Logging.D("ViewPost: Loaded.");

            if (viewModel.Post == null)
                throw new Exception();

            // DO NOT LOAD F**KING GIFS!!! THEY BLOW UP YOUR PHONE!!!
            if (viewModel.Post.FileExtension.ToLower() == "gif")
            {
                Logging.D("ViewPost: GIF post rejected.");
                MessageBox.Show(
                    "Loading this post is a bad idea because it will use way too much memory. We apologize for inconvenience.",
                    "We've got some problems", MessageBoxButton.OK);
                NavigationService.GoBack();
            }

            // Load Post Data
            LoadPost(viewModel.Post);
            viewModel.LoadPostCommand.Execute(null);

        }
        
        /// <summary>
        ///     Loads the post on a background thread,
        ///     but does not change the UI state.
        /// </summary>
        /// <param name="p"></param>
        private void LoadPost(Post p)
        {
            // Reset all parameters
            PivotRoot.SelectedIndex = 0;
            animation.IsEnabled = true;
            PivotRoot.IsLocked = true;
            viewModel.Comments.Clear();

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
                        foreach (string t in tags)
                        {
                            TextBlock block = new TextBlock {FontSize = TagFontSize, Tag = t};
                            block.Tap += (@s, e) =>
                                {
                                    Object tag = ((TextBlock) s).Tag;
                                    viewModel.SearchTagCommand.Execute(tag);
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
            String[] cpTags = p.CopyrightTagsString.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            loadTags(cpTags, CopyrightTagHeader, CopyrightTagPanel, Color.FromArgb(0xFF, 0xEE, 0x66, 0xEE));
            String[] chTags = p.CharacterTagsString.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            loadTags(chTags, CharacterTagHeader, CharacterTagPanel, Color.FromArgb(0xFF, 0x44, 0xEE, 0x44));
            String[] arTags = p.ArtistTagsString.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            loadTags(arTags, ArtistTagHeader, ArtistTagPanel, Color.FromArgb(0xFF, 0xEE, 0x66, 0x66));
            String[] gTags = p.GeneralTagsString.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            loadTags(gTags, GeneralTagHeader, GeneralTagPanel, Color.FromArgb(0xFF, 0x44, 0x77, 0xFF));

            // Load Post Info
            PostIdTextBlock.Text = p.ID.ToString(CultureInfo.InvariantCulture);
            UploaderTextBlock.Text = p.Author;

            // Convert Byte size to something more readable
            SizeTextBlock.Text = viewModel.Post.FileSizeString;
            RatingTextBlock.Text = p.Rating.ToString();
        }

    }
}