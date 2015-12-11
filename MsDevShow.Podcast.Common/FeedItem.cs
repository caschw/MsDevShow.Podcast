using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsDevShow.Podcast.Common
{
    public class FeedItemService
    {
        public void SetupDb()
        {

            var path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "db.sqlite");

            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path))
            {
                conn.CreateTable<FeedItem>();
            }
        }
    }
    public class FeedItem
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public DateTime PublicationDate { get; set; }
        public string GUID { get; set; }
        public string EnclosureUrl { get; set; }
        public TimeSpan Duration { get; set; }

        public bool IsDownloaded { get; set; }
    }
}
