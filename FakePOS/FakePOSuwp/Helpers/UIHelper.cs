using System;

#if WINDOWS_UWP
using Windows.UI.Xaml;
#else
using Microsoft.UI.Xaml;
#endif

namespace FakePOS.Helpers
{
    public class UIHelper
    {
        static public UIHelper Current { get; private set; }

        static UIHelper()
        {
            Current = new UIHelper();
        }

        public Visibility Visible(bool? value)
        {
            return value == true ? Visibility.Visible : Visibility.Collapsed;
        }

        public Visibility Collapsed(bool? value)
        {
            return value == false ? Visibility.Visible : Visibility.Collapsed;
        }

        public string Currency(double? value)
        {
            value = value ?? 0;
            return value.Value.ToString("0.00");
        }
    }
}
