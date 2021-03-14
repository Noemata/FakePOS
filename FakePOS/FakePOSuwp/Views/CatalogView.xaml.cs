#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
#endif
using Microsoft.Toolkit.Mvvm.DependencyInjection;

using FakePOS.ViewModels;

namespace FakePOS.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CatalogView : Page
    {
        public CatalogView()
        {
            InitializeComponent();

            DataContext = ViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            itemsGrid.Initialize();
            itemsList.Initialize();

            ViewModel.GridViewModel = itemsGrid.ViewModel;
            ViewModel.ListViewModel = itemsList.ViewModel;

            var state = (e.Parameter as CatalogState) ?? new CatalogState();
            await ViewModel.LoadAsync(state);
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            await ViewModel.UnloadAsync();
        }

        internal CatalogViewModel ViewModel = Ioc.Default.GetService<CatalogViewModel>();
    }
}
