﻿using ReactiveUI.Fody.Helpers;

namespace NowPlaying.Wpf.Controls.PlayingTrack
{
    public class ProgressViewModel
    {
        // 0 <= Progress <= 100
        [Reactive] public int Progress { get; set; }
    }
}