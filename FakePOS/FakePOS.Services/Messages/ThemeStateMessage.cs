using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace FakePOS.Messages
{
    public enum ThemeState
    {
        Light,
        Dark,
        Default
    }

    public sealed class ThemeStateMessage : AsyncRequestMessage<ThemeState>
    {

        public ThemeStateMessage(ThemeState state)
        {
            State = state;
        }

        public ThemeState State { get; }
    }
}
