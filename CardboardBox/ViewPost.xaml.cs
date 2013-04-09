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
using Microsoft.Phone.Controls;
using libWyvernzora.BarlogX.Animation;

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

            // Load Image
            BitmapImage image = new BitmapImage(new Uri("http://konachan.com/image/441f4f6645338bebf2edd5e80050cb2d/Konachan.com%20-%20157558%20hatsune_miku%20sakura_miku%20vocaloid.jpg", UriKind.Absolute));
            image.DownloadProgress += (@s, e) =>
                {
                    progressBar.Value = e.Progress;
                    if (e.Progress == 100)
                        VisualStateManager.GoToState(this, "Loaded", true);
                };
            image.ImageFailed += (@s, e) =>
                {
                    System.Diagnostics.Debugger.Break();
                };
            PostImage.Source = image;

            /*
           if (Session.Instance.SelectedPost.LargeImageSource == null)
            {
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += (@s, e) =>
                    {
                        Session.Instance.Cache.EnsureSampleCache(Session.Instance.SelectedPost);
                        e.Result = Session.Instance.Cache.GetSampleStream(Session.Instance.SelectedPost);
                    };
                bw.RunWorkerCompleted += (@s, e) =>
                    {
                        VisualStateManager.GoToState(this, "Loaded", true);
                        
                        BitmapImage img = new BitmapImage();
                        img.SetSource((Stream)e.Result);
                        Session.Instance.SelectedPost.LargeImageSource = img;
                        PostImage.Source = Session.Instance.SelectedPost.LargeImageSource;
                    };
                bw.RunWorkerAsync();
            } 
            */


        }


    }
}