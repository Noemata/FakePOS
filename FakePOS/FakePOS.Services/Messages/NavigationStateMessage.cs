using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace FakePOS.Messages
{
    public enum NavigationState
    {
        GotoShell,
        GotoLogin,
    }

    public sealed class NavigationStateMessage : AsyncRequestMessage<NavigationState>
    {

        public NavigationStateMessage(NavigationState state)
        {
            State = state;
        }

        public NavigationState State { get; }
    }
}
