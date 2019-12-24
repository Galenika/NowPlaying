using System;
using Avalonia.Controls;
using NowPlaying.Api;

namespace NowPlaying.Views
{
    public class BrowserWindow : Window
    {
        private SpotifyRequestsManager _spotify;

        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }
        public DateTime TokenExpireTime { get; private set; }

        public BrowserWindow(SpotifyRequestsManager spotify)
        {
            _spotify = spotify;
        }
    }
}