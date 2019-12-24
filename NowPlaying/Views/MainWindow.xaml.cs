using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using NowPlaying.Api;
using NowPlaying.Models;
using NowPlaying.ViewModels;

namespace NowPlaying.Views
{
    public class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        private readonly SpotifyRequestsManager _spotify;

        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);

            Hide();
            var browserWindow = new BrowserWindow(_spotify);
            browserWindow.ShowDialog(this);

            // Xilium.CefGlue.CefBrowser

            _spotify = new SpotifyRequestsManager(SpotifyConfig.ClientId, SpotifyConfig.ClientSecret);
            _spotify.AccessToken = browserWindow.AccessToken;
            _spotify.RefreshToken = browserWindow.RefreshToken;
            _spotify.TokenExpireTime = browserWindow.TokenExpireTime;

            ViewModel = new MainWindowViewModel(_spotify);
        }
    }
}