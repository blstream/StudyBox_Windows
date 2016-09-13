﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace StudyBox
{
    public class IsXboxTrigger : StateTriggerBase
    {
        public IsXboxTrigger()
        {
            SetActive(App.IsXbox);

            App.IsXboxModeChanged += App_TenFootModeChanged;
        }

        private void App_TenFootModeChanged(object sender, EventArgs e)
        {
            SetActive(App.IsXbox);
        }
    }
}
