#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
#endif

namespace FakePOS.Controls
{
    public class ToggleButtonControl : ToggleButton
    {
        public string PathSource
        {
            get => (string)GetValue(PathSourceProperty);
            set => SetValue(PathSourceProperty, value);
        }

        public static readonly DependencyProperty PathSourceProperty =
            DependencyProperty.Register("PathSource", typeof(string), typeof(ToggleButtonControl), new PropertyMetadata(string.Empty));
    }
}
