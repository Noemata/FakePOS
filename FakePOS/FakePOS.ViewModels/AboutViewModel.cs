using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using FakePOS.Services;
using Microsoft.Toolkit.Mvvm.Messaging;
using FakePOS.Messages;

namespace FakePOS.ViewModels
{
    [RegisterWithIoc(InstanceMode.Transient)]
    public class AboutViewModel : ObservableRecipient
    {
        private readonly IMessenger _messenger;

        public AboutViewModel(IMessenger messenger)
        {
            _messenger = messenger;

            // MP! todo: figure out how to set theme globally, implementation not yet complete
            //_messenger.Send(new ThemeStateMessage(ThemeState.Light));
        }
    }
}
