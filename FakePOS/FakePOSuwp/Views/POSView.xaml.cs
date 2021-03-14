#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endif
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using FakePOS.ViewModels;

namespace FakePOS.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class POSView : Page
    {
        public POSView()
        {
            InitializeComponent();
        }

        internal POSViewModel ViewModel => Ioc.Default.GetService<POSViewModel>();
    }
}
