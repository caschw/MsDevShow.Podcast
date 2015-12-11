using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MsDevShow.Podcast.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MsDevShow.Podcast
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            PageHelpers.ThemeTitleBar();
            
            this.InitializeComponent();
            var feeds = GetFeeds();
            DataContext = feeds;    
        }

        private List<FeedItem> GetFeeds()
        {
            var http = new HttpClient();
            var rss = http.GetStringAsync(AppSettings.EpisodeRssFeed).Result;

            var feeds = ShowFeed.ParseRssFeed(rss).ToList();
            return feeds;
        }

        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var t = (FeedItem)((TextBlock)e.OriginalSource).DataContext;
            Frame.Navigate(typeof(EpisodeDetailPage), t);
        }
    }
}
