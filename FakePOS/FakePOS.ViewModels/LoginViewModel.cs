using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using FakePOS.Messages;
using FakePOS.Services;

namespace FakePOS.ViewModels
{
    [RegisterWithIoc(InstanceMode.Transient)]
    public class LoginViewModel : ObservableRecipient
    {
        private readonly IMessenger _messenger;

        private string _username;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set => SetProperty(ref _password, value);
        }

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public IRelayCommand LoginCommand { get; }

        public LoginViewModel(IMessenger messenger)
        {
            _messenger = messenger;

            LoginCommand = new RelayCommand(Login);

            Message = "Hello home.";
        }

        private void Login()
        {
            _messenger.Send(new NavigationStateMessage(NavigationState.GotoShell));
        }
    }
}
