﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Shell;

namespace CardboardBox.UI
{
    public class AppBarButton : ApplicationBarIconButton
    {
        public AppBarButton()
        {
            Click += (@s, e) => Command.Execute(CommandParam);
        }

        public ICommand Command { get; set; }

        public Object CommandParam { get; set; }
    }
}
