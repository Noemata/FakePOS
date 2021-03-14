using System;
#if WINDOWS_UWP
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;
#else
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Media.Animation;
#endif

namespace FakePOS.Services
{
    public interface INavigationService
    {
        event NavigatedEventHandler Navigated;
        event NavigationFailedEventHandler NavigationFailed;
        void Initialize(Frame shellFrame);
        bool IsShell();
        bool CanGoBack();
        bool CanGoForward();
        bool GoBack();
        void GoForward();
        void RemoveFromBackStack();
        bool Navigate(Type pageType, object parameter = null, NavigationTransitionInfo infoOverride = null);
        public bool Navigate<T>(object parameter = null, NavigationTransitionInfo infoOverride = null) where T : Page;
    }

}
