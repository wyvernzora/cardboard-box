using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using CardboardBox.API;
using CardboardBox.Model;
using libDanbooru2;

namespace CardboardBox.UI
{
    /// <summary>
    /// Utility class to aid in listbox virtualization
    /// </summary>
    public class PostTuple
    {
        private static ICommand viewCommand;
        public static void SetViewCommand(ICommand command)
        {
            viewCommand = command;
        }

        public ICommand ViewCommand
        { get { return viewCommand; } }

        public Post First { get; set; }

        public Visibility FirstVisibility
        { get { return First == null ? Visibility.Collapsed : Visibility.Visible; } }

        public Post Second { get; set; }

        public Visibility SecondVisibility
        { get { return Second == null ? Visibility.Collapsed : Visibility.Visible; } }

        public Post Third { get; set; }

        public Visibility ThirdVisibility
        { get { return Third == null ? Visibility.Collapsed : Visibility.Visible; } }
    }
}
