using System;

#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endif

namespace FakePOS
{
    public static class FrameExtensions
    {
        public static TResult TryGetResource<TResult>(this Frame frame, string key, TResult defaultValue = default(TResult))
        {
            if (frame.Content is FrameworkElement ui)
            {
                if (ui.Resources.ContainsKey(key))
                {
                    return (TResult)ui.Resources[key];
                }
            }
            return defaultValue;
        }
    }
}
