using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace FakePOS.Messages
{
    public enum ShellState
    {
        BusyOn,
        BusyOff,
        VersionOn,
        VersionOff,
    }

    public sealed class ShellStateMessage : AsyncRequestMessage<ShellState>
    {

        public ShellStateMessage(ShellState state)
        {
            State = state;
        }

        public ShellState State { get; }
    }
}
