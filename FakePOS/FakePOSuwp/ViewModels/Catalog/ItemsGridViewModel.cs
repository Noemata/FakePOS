using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Controls;
#endif

using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

using FakePOS.Models;
using FakePOS.Services;
using FakePOS.Providers;
using FakePOS.Views;

namespace FakePOS.ViewModels
{
    public enum GridCommandBarMode
    {
        Idle,
        MultiSelect,
        ItemsSelected,
        AllSelected
    }

    [RegisterWithIoc(InstanceMode.Transient)]
    public class ItemsGridViewModel : ObservableRecipient
    {
        private readonly IUserNotificationService _userNotificationService;
        private readonly IMessenger _messenger;

        public ItemsGridViewModel(ICatalogProvider catalogProvider, IMessenger messenger, IUserNotificationService userNotificationService)
        {
            DataProvider = catalogProvider;
            _userNotificationService = userNotificationService;
            _messenger = messenger;

            _barItems = new ObservableCollection<CatalogItemModel>();
        }

        public ICatalogProvider DataProvider { get; }

        public CatalogState State { get; set; }

        public GridView ItemsControl { get; set; }
        public GridView BarItemsControl { get; set; }

        public bool IsActive { get; set; }

        public GridCommandBarMode Mode { get; set; }

        private ObservableCollection<CatalogItemModel> _items = null;
        public ObservableCollection<CatalogItemModel> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        private ObservableCollection<CatalogItemModel> _barItems = null;
        public ObservableCollection<CatalogItemModel> BarItems
        {
            get { return _barItems; }
        }

        private ListViewSelectionMode _selectionMode = ListViewSelectionMode.None;
        public ListViewSelectionMode SelectionMode
        {
            get { return _selectionMode; }
            set { SetProperty(ref _selectionMode, value); }
        }

        private bool _isCommandBarOpen = false;
        public bool IsCommandBarOpen
        {
            get { return _isCommandBarOpen; }
            set { SetProperty(ref _isCommandBarOpen, value); }
        }

        public bool IsMultiSelectVisible => Mode == GridCommandBarMode.Idle;
        public bool IsSelectAllVisible => Mode == GridCommandBarMode.MultiSelect || Mode == GridCommandBarMode.ItemsSelected;
        public bool IsClearVisible => Mode == GridCommandBarMode.AllSelected;
        public bool IsCancelVisible => Mode != GridCommandBarMode.Idle;
        public bool IsSeparatorVisible => IsDeleteVisible;
        public bool IsDeleteVisible => Mode == GridCommandBarMode.ItemsSelected || Mode == GridCommandBarMode.AllSelected;

        public IRelayCommand ItemClickCommand => new RelayCommand<CatalogItemModel>(OnItemClick);
        public IRelayCommand PreSelectCommand => new RelayCommand(OnMultiSelect);
        public IRelayCommand MultiSelectCommand => new RelayCommand(OnMultiSelect);
        public IRelayCommand SelectAllCommand => new RelayCommand(OnSelectAll);
        public IRelayCommand ClearCommand => new RelayCommand(OnClear);
        public IRelayCommand CancelCommand => new RelayCommand(OnCancel);
        public IRelayCommand DeleteCommand => new RelayCommand(OnDelete);

        private void SelecteAll() => ItemsControl.SelectRange(new ItemIndexRange(0, (uint)Items.Count));
        private void DeselectAll() => ItemsControl.DeselectRange(new ItemIndexRange(0, (uint)Items.Count));

        public void UpdateExternalSelection()
        {
            _cancelOnSelectionChanged = true;

            if (Items != null)
            {
                BarItems.Clear();
                foreach (var item in Items.Where(r => r.IsSelected))
                {
                    BarItems.Add(item);
                }

                int selectedCount = Items.Count(r => r.IsSelected);
                if (selectedCount > 0)
                {
                    // Set SelectionMode = Multiple before selecting items
                    SelectionMode = ListViewSelectionMode.Multiple;
                    if (selectedCount < Items.Count)
                    {
                        foreach (var item in Items)
                        {
                            if (ItemsControl.ContainerFromItem(item) is GridViewItem container)
                            {
                                container.IsSelected = item.IsSelected;
                            }
                        }
                    }
                    else
                    {
                        SelecteAll();
                    }
                }
                else
                {
                    Mode = GridCommandBarMode.Idle;
                    DeselectAll();
                }
                UpdateCommandBar();
            }

            _cancelOnSelectionChanged = false;
        }

        private bool _cancelOnSelectionChanged = false;

        public async void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (_cancelOnSelectionChanged)
            {
                return;
            }

            foreach (CatalogItemModel item in args.AddedItems)
            {
                AddBarItem(item);
                item.IsSelected = true;
            }

            if (args.AddedItems.Count + args.RemovedItems.Count == 1)
            {
                await UpdateCommandBarItems(args);
            }

            foreach (CatalogItemModel item in args.RemovedItems)
            {
                RemoveBarItem(item);
                item.IsSelected = false;
            }

            UpdateCommandBar();
        }

        private void OnItemClick(CatalogItemModel item)
        {
            if (SelectionMode == ListViewSelectionMode.None)
            {
                ItemsControl.PrepareConnectedAnimation("ItemSelected", item, "imageContainer");
                ShellViewModel.Navigate(typeof(ItemDetailView), new ItemDetailState(item));

                State.SelectedItemId = item.Id;
            }
        }

        private void OnMultiSelect()
        {
            Mode = GridCommandBarMode.MultiSelect;
            IsCommandBarOpen = true;
            UpdateCommandBar();
        }

        private void OnSelectAll()
        {
            ApplySelection(Items, true);
            SelecteAll();
            Mode = GridCommandBarMode.AllSelected;
            IsCommandBarOpen = true;
            UpdateCommandBar();
        }

        private void OnClear()
        {
            ApplySelection(Items, false);
            DeselectAll();
            SelectionMode = ListViewSelectionMode.Multiple;
            Mode = GridCommandBarMode.MultiSelect;
            IsCommandBarOpen = true;
            UpdateCommandBar();
        }

        private void OnCancel()
        {
            ApplySelection(Items, false);
            DeselectAll();
            SelectionMode = ListViewSelectionMode.None;
            Mode = GridCommandBarMode.Idle;
            IsCommandBarOpen = false;
            UpdateCommandBar();
        }

        private async void OnDelete()
        {
            // MP! fixme: not yet implemented
            //ShellViewModel.Current.EnableView(false);

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
                        BarItems.Remove(item);
                    }
                    SelectionMode = ListViewSelectionMode.None;
                    Mode = GridCommandBarMode.Idle;

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
            // MP! fixme: not yet implemented
            //ShellViewModel.Current.EnableView(true);

            IsCommandBarOpen = false;
            UpdateCommandBar();
        }

        public void UpdateCommandBar()
        {
            int count = Items.Count(r => r.IsSelected);
            if (count > 0)
            {
                if (count < Items.Count)
                {
                    Mode = GridCommandBarMode.ItemsSelected;
                }
                else
                {
                    Mode = GridCommandBarMode.AllSelected;
                }
            }

            SelectionMode = Mode == GridCommandBarMode.Idle ? ListViewSelectionMode.None : ListViewSelectionMode.Multiple;

            OnPropertyChanged("IsMultiSelectVisible");
            OnPropertyChanged("IsSelectAllVisible");
            OnPropertyChanged("IsClearVisible");
            OnPropertyChanged("IsCancelVisible");
            OnPropertyChanged("IsSeparatorVisible");
            OnPropertyChanged("IsDeleteVisible");

            OnPropertyChanged("Name");
            OnPropertyChanged("DataProvider.Name");
            OnPropertyChanged("CatalogTypes");
        }

        private void ApplySelection(IEnumerable<object> items, bool isSelected)
        {
            foreach (CatalogItemModel item in items)
            {
                item.IsSelected = isSelected;
            }
        }

        private void AddBarItem(CatalogItemModel item)
        {
            if (!BarItems.Contains(item))
            {
                BarItems.Add(item);
            }
        }

        private void RemoveBarItem(CatalogItemModel item)
        {
            if (BarItems.Contains(item))
            {
                BarItems.Remove(item);
            }
        }

        private async Task UpdateCommandBarItems(SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count == 1)
            {
                IsCommandBarOpen = true;
                var item = args.AddedItems[0];
                var anim = ItemsControl.PrepareConnectedAnimation("AddedItems", item, "imageContainer");
                await Task.Delay(100);
                BarItemsControl.ScrollIntoView(item);
                await BarItemsControl.TryStartConnectedAnimationAsync(anim, item, "image");
            }
            else if (args.RemovedItems.Count == 1)
            {
                IsCommandBarOpen = true;
                var item = args.RemovedItems[0];
                var anim = BarItemsControl.PrepareConnectedAnimation("RemovedItems", item, "image");
                await Task.Delay(100);
                BarItemsControl.ScrollIntoView(item);
                await ItemsControl.TryStartConnectedAnimationAsync(anim, item, "imageContainer");
            }
        }
    }
}
