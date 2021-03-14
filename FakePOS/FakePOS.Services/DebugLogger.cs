using System.Diagnostics;

namespace FakePOS.Services
{
    public class DebugLogger : ILoggingService
    {
        public void Log(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
