using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace CardboardBox
{
    public partial class ViewPost : PhoneApplicationPage
    {
        public ViewPost()
        {
            InitializeComponent();

            // Load Barlox Animation
            StreamResourceInfo animData = Application.GetResourceStream(new Uri("Assets/chibi-small.bxa", UriKind.Relative));
            BarloxAnimation animation = new BarloxAnimation(animData.Stream);
            animation.FrameChanged += (@s, e) =>
            {
                LoadingAnimationImage.Source = e.NewFrame.Source;
            };
            animation.IsEnabled = true;

            /*
           if (Session.Instance.SelectedPost.LargeImageSource == null)
            {
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += (@s, e) =>
                    {
                        Session.Instance.Cache.EnsureSampleCache(Session.Instance.SelectedPost);
                    };
                bw.RunWorkerCompleted += (@s, e) =>
                    {
                        VisualStateManager.GoToState(this, "Loaded", true);
                        Session.Instance.SelectedPost.LargeImageSource =
                            Session.Instance.Cache.GetTile(Session.Instance.SelectedPost);
                        PostImage.Source = Session.Instance.SelectedPost.LargeImageSource;
                    };
                bw.RunWorkerAsync();
            } 
             * */


        }


    }
}