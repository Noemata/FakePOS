using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

using FakePOS.Views;
using FakePOS.Services;
using FakePOS.Messages;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FakePOS
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            var rootFrame = Content as Frame;

            var messenger = Ioc.Default.GetService<IMessenger>();
            var navigationService = Ioc.Default.GetService<INavigationService>();
            navigationService.Initialize(rootFrame);
            navigationService.Navigate<LoginView>();

            messenger.Register<NavigationStateMessage>(this, (r, m) =>
            {
                if (m.State == NavigationState.GotoLogin)
                    navigationService.Navigate<LoginView>();

                if (m.State == NavigationState.GotoShell)
                    navigationService.Navigate<ShellView>();

                m.Reply(m.State);
            });

            messenger.Register<ThemeStateMessage>(this, (r, m) =>
            {
                if (m.State == ThemeState.Light)
                    rootFrame.RequestedTheme = ElementTheme.Light;

                if (m.State == ThemeState.Dark)
                    rootFrame.RequestedTheme = ElementTheme.Dark;

                if (m.State == ThemeState.Default)
                    rootFrame.RequestedTheme = ElementTheme.Default;

                m.Reply(m.State);
            });
        }
    }
}
