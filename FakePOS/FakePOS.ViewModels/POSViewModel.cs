using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using FakePOS.Services;

namespace FakePOS.ViewModels
{
    [RegisterWithIoc(InstanceMode.Transient)]
    public class POSViewModel : ObservableRecipient
    {
        public POSViewModel()
        {
        }
    }
}
