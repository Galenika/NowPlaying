﻿using ReactiveUI;
using System.Reactive.Disposables;

namespace NowPlaying.Wpf.Controls.PlayingTrack
{
    public class PlayingTrackControlBase : ReactiveUserControl<PlayingTrackViewModel>
    {
        //
    }

    public partial class PlayingTrackControl : PlayingTrackControlBase
    {
        public PlayingTrackControl()
        {
            ViewModel = new PlayingTrackViewModel();

            InitializeComponent();

            this.WhenActivated(d => {
                this.OneWayBind(ViewModel, vm => vm.Author, v => v.TrackAuthorTextBlock.Text)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Title, v => v.TrackNameTextBlock.Text)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel,
                        vm => vm.ProgressMs, v => v.Progress.ViewModel.Progress,
                        progressMs => GetProgess(progressMs, ViewModel.DurationMs))
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.CurrentProgress, v => v.CurrentProgress.Text)
                     .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.EstimatedProgress, v => v.EstimatedProgress.Text)
                     .DisposeWith(d);
            });
        }

        private long GetProgess(long progressMs, long durationMs)
        {
            return durationMs == 0 ? 0 : progressMs / durationMs / 100;
        }
    }
}
