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
    public sealed partial class ShellView : Page
    {
        public ShellView()
        {
            InitializeComponent();
            // MP! todo: resolve putting selection state in ShellViewModel
            NavigationView.SelectedItem = NavigationView.MenuItems[0];
        }

        private bool bNot(bool value) => value ? false : true;
        private Visibility vNot(bool value) => value ? Visibility.Collapsed : Visibility.Visible;

        internal ShellViewModel ViewModel = Ioc.Default.GetService<ShellViewModel>();
    }
}
