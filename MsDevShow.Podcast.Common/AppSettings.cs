using System;
using System.Collections.Generic;
using Windows.UI;

namespace MsDevShow.Podcast.Common
{
    public static class AppSettings
    {
        public static Color MsDevShowBlue => Color.FromArgb(255, 50, 159, 217);

        public static string MsDevShowBlueHex => "329FD9";

        public static Color MsDevShowRed => Color.FromArgb(255, 239, 81, 43);

        public static string MsDevShowRedHex => "EF512B";

        public static string EpisodeRssFeed => "http://msdevshow.libsyn.com/rss";

        public static string PodcastTitle => "MS Dev Show";

        public static string PodcastDescription => "A NEW podcast for Microsoft developers covering topics such as Azure/cloud, Windows, Windows Phone, .NET, Visual Studio, and more! Hosted by Jason Young and Carl Schweitzer.";

        public static IDictionary<string, Uri> MainScreenWebPages()
        {
            return new Dictionary<string, Uri>
            {
                { "Twitter", new Uri("http://twitter.com/msdevshow") }
            };
        }
    }
}
