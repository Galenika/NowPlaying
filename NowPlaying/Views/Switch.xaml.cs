using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Reactive.Linq;
using System;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Disposables;

namespace NowPlaying.Views
{
    public class SwitchModel : ReactiveObject
    {
        [Reactive]
        public bool IsToggled { get; set; }

        public SwitchModel()
        {
            this.WhenAnyValue(x => x.IsToggled);
        }

        public void Toggle()
        {
            IsToggled = !IsToggled;
        }
    }

    public class Switch : ReactiveUserControl<SwitchModel>
    {
        private readonly Ellipse _ellipse;
        private readonly Border _rect;

        public bool IsToggled => _ellipse.HorizontalAlignment == HorizontalAlignment.Right;

        public Switch()
        {
            AvaloniaXamlLoader.Load(this);

            _ellipse = this.FindControl<Ellipse>("Ellipse");
            _rect = this.FindControl<Border>("Rect");

            ViewModel = new SwitchModel();

            var getHashCode = ReactiveCommand.Create(() => GetHashCode());

            this.WhenActivated(d =>
            {
                var pointerPressed = Observable.FromEventPattern<RoutedEventArgs>(_rect, "PointerPressed");
                pointerPressed.Subscribe(evt => ToggleEllipse()).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.IsToggled, v => v.IsToggled).DisposeWith(d);
            });
        }

        public void ToggleEllipse()
        {
            _ellipse.HorizontalAlignment = IsToggled
                                         ? HorizontalAlignment.Left
                                         : HorizontalAlignment.Right;
        }
    }
}
