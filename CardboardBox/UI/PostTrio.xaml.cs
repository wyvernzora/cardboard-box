using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using CardboardBox.UI;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace CardboardBox
{
    public partial class PostTrio : UserControl
    {
        public static readonly DependencyProperty TupleProperty
            = DependencyProperty.Register("Tuple", typeof(PostTuple), typeof(PostTrio), new PropertyMetadata(null, TupleChanged));

        public PostTrio()
        {
            InitializeComponent();
        }

        public PostTuple Tuple
        {
            get { return (PostTuple)GetValue(TupleProperty); }
            set
            {
                SetValue(TupleProperty, value);
            }
        }

        private static void TupleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PostTrio trio = (PostTrio) o;
            PostTuple value = trio.Tuple;

            if (value == null)
                {
                    trio.Visibility = Visibility.Collapsed;
                    return;
                }

                if (value.First != null)
                {
                    trio.FirstImage.Source = new BitmapImage(value.First.PreviewUrl);
                    trio.FirstBorder.Visibility = Visibility.Visible;
                }
                else
                    trio.FirstBorder.Visibility = Visibility.Collapsed;

                if (value.Second != null)
                {
                    trio.SecondImage.Source = new BitmapImage(value.Second.PreviewUrl);
                    trio.SecondBorder.Visibility = Visibility.Visible;
                }
                else
                    trio.SecondBorder.Visibility = Visibility.Collapsed;

                if (value.Third != null)
                {
                    trio.ThirdImage.Source = new BitmapImage(value.Third.PreviewUrl);
                    trio.ThirdBorder.Visibility = Visibility.Visible;
                }
                else
                    trio.ThirdBorder.Visibility = Visibility.Collapsed;
        }
    }
}