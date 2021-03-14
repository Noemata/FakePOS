using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endif

using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

using FakePOS.Models;
using FakePOS.Services;
using FakePOS.Providers;

namespace FakePOS.ViewModels
{
    public enum ListCommandBarMode
    {
        Idle,
        ItemsSelected,
        AllSelected
    }

    [RegisterWithIoc(InstanceMode.Transient)]
    public class ItemsListViewModel : ObservableRecipient
    {
        private readonly IUserNotificationService _userNotificationService;
        private readonly IMessenger _messenger;

        public ItemsListViewModel(ICatalogProvider catalogProvider, IMessenger messenger, IUserNotificationService userNotificationService)
        {
            _userNotificationService = userNotificationService;
            _messenger = messenger;

            DataProvider = catalogProvider;
        }

        public ICatalogProvider DataProvider { get; }

        public CatalogState State { get; set; }

        public DataGrid ItemsControl { get; set; }

        public bool IsActive { get; set; }

        public ListCommandBarMode Mode { get; set; }

        private ObservableCollection<CatalogItemModel> _items = null;
        public ObservableCollection<CatalogItemModel> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        private IList<CatalogTypeModel> _catalogTypes = null;
        public IList<CatalogTypeModel> CatalogTypes
        {
            get { return _catalogTypes; }
            set { SetProperty(ref _catalogTypes, value); }
        }

        private IList<CatalogBrandModel> _catalogBrands = null;
        public IList<CatalogBrandModel> CatalogBrands
        {
            get { return _catalogBrands; }
            set { SetProperty(ref _catalogBrands, value); }
        }

        private bool _isCommandBarOpen = false;
        public bool IsCommandBarOpen
        {
            get { return _isCommandBarOpen; }
            set { SetProperty(ref _isCommandBarOpen, value); }
        }

        public bool IsSelectAllVisible => Mode != ListCommandBarMode.AllSelected;
        public bool IsClearVisible => Mode == ListCommandBarMode.AllSelected;
        public bool IsCancelVisible => Mode != ListCommandBarMode.Idle;
        public bool IsSeparatorVisible => IsDeleteVisible;
        public bool IsDeleteVisible => Mode == ListCommandBarMode.ItemsSelected || Mode == ListCommandBarMode.AllSelected;

        // MP! fixme: I think this is ok now, veryify ...
        public IRelayCommand SelectionChangedCommand => new RelayCommand<SelectionChangedEventArgs>(OnSelectionChanged);

        public IRelayCommand SelectAllCommand => new RelayCommand(OnSelectAll);
        public IRelayCommand ClearCommand => new RelayCommand(OnClear);
        public IRelayCommand CancelCommand => new RelayCommand(OnCancel);
        public IRelayCommand DeleteCommand => new RelayCommand(OnDelete);

        public void UpdateExternalSelection()
        {
            foreach (var item in Items.Where(r => r.IsSelected))
            {
                // MP! fixme:
                //ItemsControl.SelectItem(item);
                ItemsControl.SelectedItem = item;
            }
            foreach (var item in Items.Where(r => !r.IsSelected))
            {
                // MP! fixme:
                //ItemsControl.DeselectItem(item);
                
            }
        }

        private bool _cancelOnSelectionChanged = false;

        // MP! fixme:
        private void OnSelectionChanged(SelectionChangedEventArgs args)
        {
            if (_cancelOnSelectionChanged)
            {
                return;
            }

            if (Items != null)
            {
                ApplySelection(args.AddedItems, true);
                ApplySelection(args.RemovedItems, false);

                int count = Items.Count(r => r.IsSelected);
                if (count == 0)
                {
                    IsCommandBarOpen = false;
                    Mode = ListCommandBarMode.Idle;
                }
                else if (count < Items.Count)
                {
                    IsCommandBarOpen = true;
                    Mode = ListCommandBarMode.ItemsSelected;
                }
                else
                {
                    IsCommandBarOpen = true;
                    Mode = ListCommandBarMode.AllSelected;
                }

                UpdateCommandBar();
            }
        }

        private void OnSelectAll()
        {
            ApplySelection(Items, true);
            // MP! fixme:
            //ItemsControl.SelectAll();
            Mode = ListCommandBarMode.AllSelected;
            IsCommandBarOpen = true;
            UpdateCommandBar();
        }

        private void OnClear()
        {
            ApplySelection(Items, false);
            // MP! fixme:
            //ItemsControl.DeselectAll();
            ItemsControl.SelectedIndex = -1;
            Mode = ListCommandBarMode.Idle;
            IsCommandBarOpen = true;
            UpdateCommandBar();
        }

        private void OnCancel()
        {
            ApplySelection(Items, false);
            // MP! fixme:
            //ItemsControl.DeselectAll();
            Mode = ListCommandBarMode.Idle;
            IsCommandBarOpen = false;
            UpdateCommandBar();
        }

        private async void OnDelete()
        {
            bool? result = await _userNotificationService.ConfirmationDialogAsync("Confirm Delete", "Ok", "Cancel");
            //if (await DialogBox.ShowAsync("Confirm Delete", "Are you sure you want to delete selected items?", "Ok", "Cancel"))

            if (result == true)
            {
                _cancelOnSelectionChanged = true;
                try
                {
                    var selectedItems = Items.Where(r => r.IsSelected).ToArray();
                    foreach (var item in selectedItems)
                    {
                        await DataProvider.DeleteItemAsync(item);
                        Items.Remove(item);
                    }

                    if (selectedItems.Length == 1)
                    {
                        var item = selectedItems[0];
                        // MP! fixme: not yet implemented
                        //ToastNotificationsService.Current.ShowToastNotification(Constants.NotificationDeletedItemTitleKey.GetLocalized(), item);
                    }
                }
                catch (Exception ex)
                {
                    //await DialogBox.ShowAsync("Error deleting files", ex);
                    await _userNotificationService.MessageDialogAsync("Error deleting files:", ex.Message);
                }
                _cancelOnSelectionChanged = false;
            }

            IsCommandBarOpen = false;
            UpdateCommandBar();
        }

        public void UpdateCommandBar()
        {
            OnPropertyChanged("IsSelectAllVisible");
            OnPropertyChanged("IsClearVisible");
            OnPropertyChanged("IsCancelVisible");
            OnPropertyChanged("IsSeparatorVisible");
            OnPropertyChanged("IsDeleteVisible");
        }

        private void ApplySelection(IEnumerable<object> items, bool isSelected)
        {
            foreach (CatalogItemModel item in items)
            {
                item.IsSelected = isSelected;
            }
        }
    }
}
