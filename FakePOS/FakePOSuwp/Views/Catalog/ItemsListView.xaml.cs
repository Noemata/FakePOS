#if WINDOWS_UWP
using Windows.UI.Xaml.Controls;
#else
using Microsoft.UI.Xaml.Controls;
#endif

using Microsoft.Toolkit.Mvvm.DependencyInjection;
using FakePOS.ViewModels;

namespace FakePOS.Views
{
    public sealed partial class ItemsListView : UserControl
    {
        public ItemsListView()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
        }

        internal ItemsListViewModel ViewModel = Ioc.Default.GetService<ItemsListViewModel>();

        public void Initialize()
        {
            ViewModel.ItemsControl = gridView;
        }
    }
}
