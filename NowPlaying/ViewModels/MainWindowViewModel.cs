
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive.Linq;
using System.Collections.ObjectModel;
using NowPlaying.Models;
using NowPlaying.Api;

namespace NowPlaying.ViewModels
{
    public class TrackInfo
    {
        public string Id { get; set; }
        public string Author { get; set; }
        public string Name { get; set; }
        public int DurationSeconds { get; set; }
    }

    public class MainWindowViewModel : ViewModelBase
    {
        private readonly SpotifyRequestsManager _spotify;

        public MainWindowViewModel(SpotifyRequestsManager spotify)
        {
            _spotify = spotify;

            Observable.Interval(TimeSpan.FromSeconds(1))
                      .Subscribe(_ => Refresh());
        }

        public void Refresh()
        {
            // заглушка
            // PlayedSeconds++;

            var trackResp = _spotify.GetCurrentTrack("");

            if (trackResp != null && trackResp.Id != this.PlayingTrack.Id)
            {

            }
        }

        [Reactive]
        public bool IsDarkTheme { get; set; }

        [Reactive]
        public bool IsRunning { get; set; }

        [Reactive]
        public TrackInfo PlayingTrack { get; set; }

        [Reactive]
        public int PlayedSeconds { get; set; }

        [Reactive]
        public bool IsAutoSendingOnNewTrack { get; set; }

        [Reactive]
        public string SelectedAccountName { get; set; }

        [Reactive]
        public string ChatButton { get; set; }
    }
}
