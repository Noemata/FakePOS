using System;
using System.Threading.Tasks;
#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
#else
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
#endif

using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

using MSWinUI = Microsoft.UI.Xaml.Controls;

using FakePOS.Views;
using FakePOS.Services;
using FakePOS.Messages;

namespace FakePOS.ViewModels
{
    [RegisterWithIoc(InstanceMode.Transient)]
    public class ShellViewModel : ObservableRecipient
    {
        private static INavigationService _navigationService;
        private readonly IUserNotificationService _userNotificationService;
        private readonly ISettingsService _settingsService;
        private readonly IMessenger _messenger;

        private string _header = "POS";
        public string Header
        {
            get => _header;
            set => SetProperty(ref _header, value);
        }

        private bool _isBusy = false;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private bool _isSetting = false;
        public bool IsSetting
        {
            get => _isSetting;
            set => SetProperty(ref _isSetting, value);
        }

        private bool _showVersion;
        public bool ShowVersion
        {
            get => _showVersion;
            set => SetProperty(ref _showVersion, value);
        }

        public string AppVersion => App.AppVersion;

        public IRelayCommand<Frame> FrameLoadedCommand { get; }
        public IRelayCommand<MSWinUI.NavigationViewItemInvokedEventArgs> ItemInvokedCommand { get; }
        public IAsyncRelayCommand LogoutAsyncCommand { get; }


        public ShellViewModel(ISettingsService settingsService, IMessenger messenger, IUserNotificationService userNotificationService)
        {
            _settingsService = settingsService;
            _messenger = messenger;
            _userNotificationService = userNotificationService;
            _navigationService = Ioc.Default.GetService<INavigationService>();

            _showVersion = _settingsService.GetValue<bool>(SettingsKeys.ShowVersionInfo);

            FrameLoadedCommand = new RelayCommand<Frame>(SetupNavigationService);
            ItemInvokedCommand = new RelayCommand<MSWinUI.NavigationViewItemInvokedEventArgs>(ExecuteItemInvokedCommand);
            LogoutAsyncCommand = new AsyncRelayCommand(LogoutAsync);

            _messenger.Register<ShellStateMessage>(this, (r, m) =>
            {
                if (m.State == ShellState.BusyOn)
                    IsBusy = true;

                if (m.State == ShellState.BusyOff)
                    IsBusy = false;

                if (m.State == ShellState.VersionOn)
                    ShowVersion = true;

                if (m.State == ShellState.VersionOff)
                    ShowVersion = false;

                m.Reply(m.State);
            });
        }

        public static bool Navigate(Type pageType, object parameter = null, NavigationTransitionInfo infoOverride = null)
        {
            if (_navigationService != null)
                return _navigationService.Navigate(pageType, parameter, infoOverride);
            else
                return false;
        }

        public static void GoBack()
        {
            if (_navigationService != null)
                _navigationService.GoBack();
        }

        public static void RemoveFromBackStack()
        {
            if (_navigationService != null)
                _navigationService.RemoveFromBackStack();
        }

        private async Task LogoutAsync()
        {
            bool? result = await _userNotificationService.ConfirmationDialogAsync("Confirm Logout", "Yes", "No");

            if (result == true)
                await _messenger.Send(new NavigationStateMessage(NavigationState.GotoLogin));
        }

        private void SetupNavigationService(Frame frame)
        {
            _navigationService.Initialize(frame);
            _navigationService.Navigate(typeof(POSView), null);

            // MP! todo: show selection in navigation menu from this viewmodel, currently in code behind
        }

        private void ExecuteItemInvokedCommand(MSWinUI.NavigationViewItemInvokedEventArgs args)
        {
            if (args != null)
            {
                string option = args.InvokedItem as string;

                if (option != null)
                {
                    switch (option)
                    {
                        case "POS":
                            Header = option;
                            _navigationService.Navigate(typeof(POSView), null);
                            break;

                        case "Catalog":
                            Header = option;
                            _navigationService.Navigate(typeof(CatalogView), null);
                            break;

                        case "About":
                            Header = option;
                            _navigationService.Navigate(typeof(AboutView), null);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public async void OnSettings()
        {
            if (IsSetting)
            {
                if (_navigationService.IsShell())
                {
                    await _userNotificationService.MessageDialogAsync("Notice:", "Navigate to a page before selecting settings.");

                    IsSetting = false;
                    return;
                }

                _navigationService.Navigate(typeof(SettingsView), null);
            }
            else
            {
                _navigationService.GoBack();
            }
        }
    }
}
