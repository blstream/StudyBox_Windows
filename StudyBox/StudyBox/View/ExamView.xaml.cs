﻿using GalaSoft.MvvmLight.Messaging;
using StudyBox.Core.Messages;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StudyBox.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExamView : Page
    {
        public ExamView()
        {
            this.InitializeComponent();
            Messenger.Default.Register<StartStoryboardMessage>(this, x => StartStoryboard(x.StoryboardName));
        }

        private void StartStoryboard(string storyboardName)
        {
            var storyboard = FindName(storyboardName) as Storyboard;
            if (storyboard != null)
            {
                storyboard.Begin();
            }
        }
    }
}
