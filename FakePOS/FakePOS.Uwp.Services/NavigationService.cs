using System;
using System.Linq;
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
    public class NavigationService : INavigationService
    {
        public event NavigatedEventHandler Navigated;
        public event NavigationFailedEventHandler NavigationFailed;

        private Frame _shellFrame;
        private object _lastParameterUsed;

        public void Initialize(Frame shellFrame)
        {
            if (_shellFrame == null)
            {
                UnregisterFrameEvents();
                _shellFrame = shellFrame;
                RegisterFrameEvents();
            }
        }

        public bool IsShell()
        {
            if (_shellFrame.CurrentSourcePageType == null || _shellFrame.CurrentSourcePageType.Name.EndsWith("ShellView"))
                return true;
            else
                return false;
        }

        public bool CanGoBack() => _shellFrame.CanGoBack;

        public bool CanGoForward() => _shellFrame.CanGoForward;

        public bool GoBack()
        {
            if (CanGoBack())
            {
                _shellFrame.GoBack();
                return true;
            }

            return false;
        }

        public void GoForward() => _shellFrame.GoForward();

        public void RemoveFromBackStack()
        {
            _shellFrame.BackStack.Remove(_shellFrame.BackStack.Last());
        }

        public bool Navigate(Type pageType, object parameter = null, NavigationTransitionInfo infoOverride = null)
        {
            if (_shellFrame.Content?.GetType() != pageType || (parameter != null && !parameter.Equals(_lastParameterUsed)))
            {
                var navigationResult = _shellFrame.Navigate(pageType, parameter, infoOverride);
                if (navigationResult)
                {
                    _lastParameterUsed = parameter;
                }

                return navigationResult;
            }
            else
            {
                return false;
            }
        }

        public bool Navigate<T>(object parameter = null, NavigationTransitionInfo infoOverride = null) where T : Page
        {
            return Navigate(typeof(T), parameter, infoOverride);
        }

        private void RegisterFrameEvents()
        {
            if (_shellFrame != null)
            {
                _shellFrame.Navigated += OnFrameNavigated;
                _shellFrame.NavigationFailed += OnFrameNavigationFailed;
            }
        }

        private void UnregisterFrameEvents()
        {
            if (_shellFrame != null)
            {
                _shellFrame.Navigated -= OnFrameNavigated;
                _shellFrame.NavigationFailed -= OnFrameNavigationFailed;
            }
        }

        private void OnFrameNavigationFailed(object sender, NavigationFailedEventArgs e) => NavigationFailed?.Invoke(sender, e);

        private void OnFrameNavigated(object sender, NavigationEventArgs e) => Navigated?.Invoke(sender, e);
    }
}

