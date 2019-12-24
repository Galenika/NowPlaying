using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NowPlaying.Views
{
    public class MinimizeClose : UserControl
    {
        public MinimizeClose()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
