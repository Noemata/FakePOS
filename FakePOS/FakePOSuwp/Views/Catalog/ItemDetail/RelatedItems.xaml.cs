using System;
#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Media.Animation;
#endif

using Microsoft.Toolkit.Mvvm.DependencyInjection;
using FakePOS.ViewModels;

namespace FakePOS.Views
{
    public sealed partial class RelatedItems : UserControl
    {
        public RelatedItems()
        {
            this.InitializeComponent();
        }

        public void UpdateBindings()
        {
            Bindings.Update();
        }

        internal ItemDetailViewModel ViewModel = Ioc.Default.GetService<ItemDetailViewModel>();
    }
}
