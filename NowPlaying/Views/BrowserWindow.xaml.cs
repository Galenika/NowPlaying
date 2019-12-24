using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NowPlaying.Api;

namespace NowPlaying.Views
{
    public class BrowserWindow : Window
    {
        private readonly SpotifyRequestsManager _spotify;

        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }
        public DateTime TokenExpireTime { get; private set; }

        public BrowserWindow()
        {
        }

        public BrowserWindow(SpotifyRequestsManager spotify)
        {
            AvaloniaXamlLoader.Load(this);

            _spotify = spotify;
        }
    }
}