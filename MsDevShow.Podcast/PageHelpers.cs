using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using MsDevShow.Podcast.Common;

namespace MsDevShow.Podcast
{
    public static class PageHelpers
    {
        public static void ThemeTitleBar()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var s = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                s.ForegroundColor = AppSettings.MsDevShowBlue;
                s.BackgroundColor = AppSettings.MsDevShowRed;
                s.BackgroundOpacity = 1;
            }
            else
            {
                var t = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
                t.ForegroundColor = AppSettings.MsDevShowBlue;
                t.BackgroundColor = AppSettings.MsDevShowRed;
            }
        }
    }
}
