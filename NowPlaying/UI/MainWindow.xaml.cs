﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using NowPlaying.ApiResponses;
using NowPlaying.Extensions;

namespace NowPlaying.UI
{
    public partial class MainWindow : Window
    {
        private bool IsAutoTrackChangeEnabled { get; set; }
        private string CurrentKeyBind { get; set; }
        protected string LastPlayingTrackId { get; set; }

        private CancellationTokenSource _cancellationGetSpotifyUpdates;

        public MainWindow()
        {
            this.InitializeComponent();

            #if DEBUG
            DebugCheckBox.Visibility = Visibility.Visible;
            
            #endif
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Hide();

            var browserWindow = new OAuth.BrowserWindow();
            browserWindow.ShowDialog();

            if (browserWindow.ResultToken == null)
            {
                this.Close();
                return;
            }

            AppInfo.State.SpotifyAccessToken = browserWindow.ResultToken;
            AppInfo.State.SpotifyRefreshToken = browserWindow.RefreshToken;
            AppInfo.State.TokenExpireTime = DateTime.Now.AddSeconds(browserWindow.ExpireTime - 5);

            new TrayMenuHelper();
            TrayMenuHelper.TrayMenu.MenuItems.Add("Show", new EventHandler((_sender, _args) => ShowFromTray()));
            TrayMenuHelper.TrayMenuIcon.DoubleClick += new EventHandler((_sender, _args) => ShowFromTray());
            TrayMenuHelper.TrayMenu.MenuItems.Add("Exit", new EventHandler((_sender, _args) => ExitFromTray()));
            TrayMenuHelper.NpcWorkTrayCheckBox.Click += new EventHandler((_sender, _args) => NpcWorkCheckChange());

            this.Show();

            if (CustomComboBox.SelectedItem == null)
            {
                MessageBox.Show("Файл loginusers.vdf пуст");
                this.Close();
                return;
            }
        }

        private void ButtonDo_Click(object sender, RoutedEventArgs e)
        {
            if (AppInfo.State.TokenExpireTime < DateTime.Now)
            {
                this.ButtonDo.Content = "spotify token expired!";
                return;
            }

            var trackResp = Requests.GetCurrentTrack(AppInfo.State.SpotifyAccessToken);

            if (trackResp == null)
                return;

            this.UpdateInterfaceTrackInfo(trackResp);

            if (CustomComboBox.SelectedItem == null)
                return;

            var cfgWriter = new ConfigWriter($@"{SteamIdLooker.UserdataPath}\{this.GetSelectedAccountId().ToString()}\730\local\cfg\audio.cfg");
            cfgWriter.RewriteKeyBinding(trackResp);
        }

        private async void ToggleSwitch_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!this.SpotifySwitch.Toggled)
            {
                this.ChangeUIState(MainWindowUIState.Idle);
                this._cancellationGetSpotifyUpdates?.Cancel();
                TrayMenuHelper.NpcWorkTrayCheckBox.Checked = false;
                return;
            }

            if (!SourceKeysExtensions.SourceEngineAllowedKeys.Contains(CustomTextBox.CurrentText))
            {
                this.SpotifySwitch.TurnOff();
                MessageBox.Show("такой кнопки в кантре нет");
                return;
            }

            TextBoxToConsole.Text = $"bind \"{CustomTextBox.CurrentText}\" \"exec audio.cfg\"";

            this.ButtonDo_Click(this, null); // force first request to not wait for the Thread.Sleep(1000)

            this.ChangeUIState(MainWindowUIState.NpcWork);

            string keyboardButton = CustomComboBox.SelectedItem;
            int _SelectedAccount = CustomComboBox.SelectedIndex;
            this._cancellationGetSpotifyUpdates = new CancellationTokenSource();

            var cfgWriter = new ConfigWriter($@"{SteamIdLooker.UserdataPath}\{this.GetSelectedAccountId().ToString()}\730\local\cfg\audio.cfg");

            await Task.Factory.StartNew((/* сюда серануть keyboardButton как нибудь*/) =>
            {                           // чтобы потом его можно было использовать внутри этого блока
                while (true)
                {
                    if (CustomComboBox.SelectedIndex != _SelectedAccount)
                        this.Dispatcher.Invoke(() => AccountsListSelectionChanged());

                    Thread.Sleep(1000);

                    if (AppInfo.State.TokenExpireTime < DateTime.Now)
                    {
                        Requests.RefreshToken();
                        cfgWriter.RewriteKeyBinding("say \"spotify token expired!\"");
                    }

                    CurrentTrackResponse trackResp = Requests.GetCurrentTrack(AppInfo.State.SpotifyAccessToken);

                    this.Dispatcher.Invoke(() => this.UpdateInterfaceTrackInfo(trackResp));
                    this.Dispatcher.Invoke(() => LabelWindowHandle.Content = AppInfo.State.WindowHandle);

                    if (trackResp?.Id != this.LastPlayingTrackId)
                    {
                        cfgWriter.RewriteKeyBinding(trackResp);
                        this.LastPlayingTrackId = trackResp.Id;

                        if (IsAutoTrackChangeEnabled && Program.GameProcess.IsValid)
                            KeySender.SendInputWithAPI(CurrentKeyBind);
                    }

                    if (this._cancellationGetSpotifyUpdates.IsCancellationRequested)
                        return;
                }
            });
        }

        private void UpdateInterfaceTrackInfo(CurrentTrackResponse trackResp)
        {
            this.IsAutoTrackChangeEnabled = CustomCheckBox.IsChecked;
            this.CurrentKeyBind = CustomTextBox.CurrentText;
            this.LabelWithButton.Content = CustomTextBox.CurrentText;

            if (trackResp == null)
            {
                this.LabelFormatted.Content = "Nothing is playing!";
                return;
            }

            if (trackResp.IsLocalFile)
                this.LabelLocalFilesWarning.Visibility = Visibility.Visible;
            else
                this.LabelLocalFilesWarning.Visibility = Visibility.Collapsed;

            this.LabelArtist.Content = $"{trackResp.FormattedArtists}";
            this.LabelFormatted.Content = $"{trackResp.Name}";
            this.LabelCurrentTime.Content = $"{trackResp.ProgressMinutes.ToString()}:{trackResp.ProgressSeconds:00}";
            this.LabelEstimatedTime.Content = $"{trackResp.DurationMinutes.ToString()}:{trackResp.DurationSeconds:00}";
        }

        private int GetSelectedAccountId()
        {
            return AppInfo.State.AccountNameToSteamId3[CustomComboBox.SelectedItem];
        }

        private void AccountsListSelectionChanged()
        {
            if (this.SpotifySwitch.Toggled && this._cancellationGetSpotifyUpdates != null)
            {
                this.SpotifySwitch.TurnOff();
                this._cancellationGetSpotifyUpdates?.Cancel();
            }
        }
    
        private void ChangeUIState(MainWindowUIState idle)
        {
            switch(idle)
            {
                case MainWindowUIState.NpcWork:
                {
                    this.LabelFormatted.Visibility     = Visibility.Visible;
                }
                break;

                case MainWindowUIState.Idle:
                {
                    this.LabelLocalFilesWarning.Visibility = Visibility.Collapsed;
                    this.LabelFormatted.Visibility         = Visibility.Collapsed;
                }
                break;
            }
        }

        private void LabelSourceKeysClick(object sender, RoutedEventArgs e)
        {
            if (!SourceKeysExtensions.TryOpenSourceKeysFile())
                MessageBox.Show("не найден файл с биндами (SourceKeys.txt)");
        }

        private void ExitFromTray() => this.Close();

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TrayMenuHelper.TrayMenuIcon.Dispose();
            TrayMenuHelper.TrayMenu.Dispose();
            Program.GameProcess.Dispose();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized) //костыль для работы трея из форм в впфе
                this.Hide();
        }

        private void ShowFromTray()
        {
            this.Show();
            WindowState = WindowState.Normal;
        }

        private void NpcWorkCheckChange()
        {
            object sender = null;
            System.Windows.Input.MouseButtonEventArgs e = null;

            if (TrayMenuHelper.NpcWorkTrayCheckBox.Checked)
            {
                this.SpotifySwitch.TurnOn();
                ToggleSwitch_MouseLeftButtonDown(sender, e);
                return;
            }

            if (!TrayMenuHelper.NpcWorkTrayCheckBox.Checked)
            {
                this.SpotifySwitch.TurnOff();
                ToggleSwitch_MouseLeftButtonDown(sender, e);
                return;
            }
        }

        private void LabelHelpClick(object sender, RoutedEventArgs e) => System.Diagnostics.Process.Start("https://github.com/veselv2010/NowPlaying/blob/master/README.md");
    }
}

//-_=ICON BY SCOUTPAN_=