﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StudyBox.Navigation
{
    public class NavigationService : INavigate
    {
        private Frame Frame { get { return (Frame)Window.Current.Content; } }

        public bool Navigate(Type sourcePageType)
        {
            return Frame.Navigate(sourcePageType);
        }
        public void Navigate(Type sourcePageType, object parameter)
        {
            Frame.Navigate(sourcePageType, parameter);
        }

        public void ClearStack()
        {
            ((Frame)Window.Current.Content).BackStack.Clear();
        }


        public virtual void GoBack()
        {
            if (this.Frame != null && this.Frame.CanGoBack) this.Frame.GoBack();
        }


        public virtual bool CanGoBack()
        {
            return this.Frame != null && this.Frame.CanGoBack;
        }

    }
}
