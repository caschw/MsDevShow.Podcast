using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MsDevShow.Podcast.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MsDevShow.Podcast
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EpisodeDetailPage : Page
    {
        private SystemMediaTransportControls _transportControls = null;

        private FeedItem _feedItem;

        private StorageFile _playingEpisode;

        public EpisodeDetailPage()
        {
            PageHelpers.ThemeTitleBar();
            this.InitializeComponent();

            SetupTransportControls();
        }

        private void SetupTransportControls()
        {
            _transportControls = SystemMediaTransportControls.GetForCurrentView();

            _transportControls.ButtonPressed += TransportControls_ButtonPressed;

            _transportControls.IsEnabled = true;
            _transportControls.IsPlayEnabled = true;
            _transportControls.IsPauseEnabled = true;
            _transportControls.IsPreviousEnabled = true;
            _transportControls.IsNextEnabled = true;
            _transportControls.IsStopEnabled = true;
        }

        private async void TransportControls_PlaybackRateChangeRequested(SystemMediaTransportControls sender, PlaybackRateChangeRequestedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (args.RequestedPlaybackRate >= 0 && args.RequestedPlaybackRate <= 2)
                {
                    MediaPlayer.PlaybackRate = args.RequestedPlaybackRate;
                    _transportControls.PlaybackRate = MediaPlayer.PlaybackRate;
                }
            });
        }

        private async void TransportControls_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        MediaPlayer.Play();
                    });
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        MediaPlayer.Pause();
                    });
                    break;
                case SystemMediaTransportControlsButton.Next:
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        // TODO: Make this configurable via settings.
                        MediaPlayer.Position = MediaPlayer.Position.Add(TimeSpan.FromSeconds(45));
                    });
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        MediaPlayer.Position = MediaPlayer.Position.Add(TimeSpan.FromSeconds(-15));
                    });
                    break;
                default:
                    break;
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            _feedItem = (FeedItem)e.Parameter;
            DataContext = _feedItem;

            _feedItem.IsDownloaded = DoesFileExist(_feedItem.GUID).Result;

            // var file = await DownloadPodcast(_feedItem.EnclosureUrl);
            //LoadPodcast(file);

            if (Frame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }
        }

        private void LoadPodcast(StorageFile episodeFile)
        {
            _playingEpisode = episodeFile;
            MediaPlayer.Source = new Uri(_playingEpisode.Path);
        }

        private async Task<bool> DoesFileExist(string fileId)
        {
            var sf = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Podcasts", CreationCollisionOption.OpenIfExists);
            var file = await sf.TryGetItemAsync(fileId + ".mp3");
            return file != null;
        }

        private async Task<StorageFile> DownloadPodcast(string enclosureUrl)
        {
            var sf = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Podcasts", CreationCollisionOption.OpenIfExists);
            var file = await sf.CreateFileAsync(_feedItem.GUID + ".mp3", CreationCollisionOption.ReplaceExisting);
            var downloader = new BackgroundDownloader();
            var download = downloader.CreateDownload(new Uri(enclosureUrl), file);
            var res = await download.StartAsync();
            return file;
        }

        private void MediaPlayer_OnCurrentStateChanged(object sender, RoutedEventArgs e)
        {
            switch (MediaPlayer.CurrentState)
            {
                case MediaElementState.Closed:
                    _transportControls.PlaybackStatus = MediaPlaybackStatus.Closed;
                    break;
                case MediaElementState.Opening:
                    break;
                case MediaElementState.Buffering:
                    break;
                case MediaElementState.Paused:
                    _transportControls.PlaybackStatus = MediaPlaybackStatus.Paused;
                    break;
                case MediaElementState.Playing:
                    _transportControls.PlaybackStatus = MediaPlaybackStatus.Playing;
                    break;
                case MediaElementState.Stopped:
                    _transportControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
                    break;
            }
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            try
            {
                var timelineProperties = new SystemMediaTransportControlsTimelineProperties();
                timelineProperties.StartTime = TimeSpan.FromSeconds(0);
                timelineProperties.MinSeekTime = TimeSpan.FromSeconds(0);
                timelineProperties.Position = MediaPlayer.Position;
                timelineProperties.MaxSeekTime = MediaPlayer.NaturalDuration.TimeSpan;
                timelineProperties.EndTime = MediaPlayer.NaturalDuration.TimeSpan;

                _transportControls.UpdateTimelineProperties(timelineProperties);

            }
            catch (Exception ex)
            {
                var t = ex;
            }
        }

        private async void MediaPlayer_OnMediaOpened(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the updater.
                var updater = _transportControls.DisplayUpdater;

                var podcastNumberString = _feedItem.EnclosureUrl.Substring(_feedItem.EnclosureUrl.Length - 8, 4);
                var podcastNumber = uint.Parse(podcastNumberString);

                if (_feedItem.IsDownloaded)
                {
                    await updater.CopyFromFileAsync(MediaPlaybackType.Music, _playingEpisode);
                }
                else
                {
                    updater.Type = MediaPlaybackType.Music;
                    updater.MusicProperties.Artist = "MS Dev Show";
                    updater.MusicProperties.Title = _feedItem.Title;
                }
                updater.MusicProperties.TrackNumber = podcastNumber;

                // Add thumbnail to transport controls
                var sf = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/iTunes Cover Art.png"));
                updater.Thumbnail = RandomAccessStreamReference.CreateFromFile(sf);

                // Update the system media transport controls
                updater.Update();
            }
            catch (Exception ex)
            {
                var t = ex;
            }
        }
    }
}
