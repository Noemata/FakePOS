#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endif

namespace FakePOS.Controls
{
    public class ButtonControl : Button
    {
        public string IconSourcePath
        {
            get => (string)GetValue(IconSourcePathProperty);
            set => SetValue(IconSourcePathProperty, value);
        }

        public static readonly DependencyProperty IconSourcePathProperty =
            DependencyProperty.Register("IconSourcePath", typeof(string), typeof(ButtonControl), new PropertyMetadata(null));
    }
}
