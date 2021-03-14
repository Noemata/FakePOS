using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endif

using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.ComponentModel;

using FakePOS.Common;
using FakePOS.Models;
using FakePOS.Helpers;
using FakePOS.Services;
using FakePOS.Providers;
using FakePOS.Views;

namespace FakePOS.ViewModels
{
    [RegisterWithIoc(InstanceMode.Singleton)]
    public class ItemDetailViewModel : ObservableRecipient
    {
        private readonly IUserNotificationService _userNotificationService;
        private readonly IMessenger _messenger;

        public ItemDetailViewModel(ICatalogProvider catalogProvider, IMessenger messenger, IUserNotificationService userNotificationService)
        {
            DataProvider = catalogProvider;
            _userNotificationService = userNotificationService;
            _messenger = messenger;
        }

        public ICatalogProvider DataProvider { get; }

        public ItemDetailState State { get; private set; }

        public bool IsNewItem => Item?.Id == 0;

        public bool HasPicture => !String.IsNullOrWhiteSpace(Item?.PictureUri);

        private IList<CatalogTypeModel> _catalogTypes = null;
        public IList<CatalogTypeModel> CatalogTypes
        {
            get { return _catalogTypes; }
            set { SetProperty(ref _catalogTypes, value); }
        }

        public CatalogTypeModel CatalogType
        {
            get => Item?.CatalogType;
            set
            {
                if (Item != null)
                {
                    Item.CatalogType = value;
                }
            }
        }

        private IList<CatalogBrandModel> _catalogBrands = null;
        public IList<CatalogBrandModel> CatalogBrands
        {
            get { return _catalogBrands; }
            set { SetProperty(ref _catalogBrands, value); }
        }

        public CatalogBrandModel CatalogBrand
        {
            get => Item?.CatalogBrand;
            set
            {
                if (Item != null)
                {
                    Item.CatalogBrand = value;
                }
            }
        }

        // MP! fixme:
        //public override bool AlwaysShowHeader => false;

        private CatalogItemModel _item;
        public CatalogItemModel Item
        {
            get { return _item ?? CatalogItemModel.Empty; }
            set
            {
                //SetProperty(ref _item, value);
                _item = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<CatalogItemModel> _relatedItems = null;
        public ObservableCollection<CatalogItemModel> RelatedItems
        {
            get { return _relatedItems; }
            set { SetProperty(ref _relatedItems, value); }
        }

        private bool _isUnavailable;
        public bool IsUnavailable
        {
            get { return _isUnavailable; }
            set { SetProperty(ref _isUnavailable, value); }
        }

        private bool _isCommandBarOpen = false;
        public bool IsCommandBarOpen
        {
            get { return _isCommandBarOpen; }
            set { SetProperty(ref _isCommandBarOpen, value); }
        }

        public bool IsSaveVisible => true;
        public bool IsSeparatorVisible => IsSaveVisible && IsDeleteVisible;
        public bool IsDeleteVisible => Item?.Id != 0;

        public ICommand SaveCommand => new RelayCommand(OnSave);
        public ICommand DeleteCommand => new RelayCommand(OnDelete);
        public ICommand ShareCommand => new RelayCommand(OnShare);

        public ICommand SelectPictureCommand => new RelayCommand(OnSelectPicture);

        public void RelatedItemSelected(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is CatalogItemModel item)
            {
                ShellViewModel.Navigate(typeof(ItemDetailView), new ItemDetailState(item));
                ShellViewModel.RemoveFromBackStack();
            }
        }

        private async void OnSave()
        {
            var result = Validate();
            if (result.IsOk)
            {
                try
                {
                    bool isNew = Item.Id == 0;
                    Item.Commit();
                    await DataProvider.SaveItemAsync(Item);

                    ShellViewModel.GoBack();

                    if (isNew)
                    {
                        //ToastNotificationsService.Current.ShowToastNotification(Constants.NotificationAddedItemTitleKey.GetLocalized(), Item);
                    }
                }
                catch (Exception ex)
                {
                    await _userNotificationService.MessageDialogAsync("Error saving item:", ex.Message);
                    //await DialogBox.ShowAsync("Error saving item", ex);
                }
            }
            else
            {
                await _userNotificationService.MessageDialogAsync("Fault:", result.Description);
                //await DialogBox.ShowAsync(result);
            }
        }

        private async void OnDelete()
        {
            bool? result = await _userNotificationService.ConfirmationDialogAsync("Confirm Delete", "Are you sure you want to delete this item?", "Ok", "Cancel");
            //await DialogBox.ShowAsync("Confirm Delete", "Are you sure you want to delete this item?", "Ok", "Cancel")

            if (result == true)
            {
                try
                {
                    await DataProvider.DeleteItemAsync(Item);
                    ShellViewModel.GoBack();

                    // MP! fixme: not yet implemented
                    //ToastNotificationsService.Current.ShowToastNotification(Constants.NotificationDeletedItemTitleKey.GetLocalized(), Item);
                }
                catch (Exception ex)
                {
                    await _userNotificationService.MessageDialogAsync("Error deleting item:", ex.Message);
                    //await DialogBox.ShowAsync("Error deleting item", ex);
                }
            }
        }

        private void OnShare()
        {
            // MP! fixme: not yet implemented
            //NavigationService.Navigate(typeof(Views.ItemShareView), new ItemShareState(Item));
        }

        private async void OnSelectPicture()
        {
            var result = await ImagePicker.OpenAsync();

            if (result != null)
            {
                Item.Picture = result.ImageBytes;
                Item.PictureFileName = result.FileName;
                Item.PictureUri = result.ImageUri;
                Item.PictureContentType = result.ContentType;
            }
        }

        private Result Validate()
        {
            if (String.IsNullOrEmpty(Item.Name))
            {
                return Result.Error("Validation error", "Name cannot be null.");
            }
            if (Item.CatalogType == null || Item.CatalogType.Id < 1)
            {
                return Result.Error("Validation error", "Catalog type cannot be empty.");
            }
            if (Item.CatalogBrand == null || Item.CatalogBrand.Id < 1)
            {
                return Result.Error("Validation error", "Catalog brand cannot be empty.");
            }
            if (!(Item.Price > 0))
            {
                return Result.Error("Validation error", "Price must be greater than zero.");
            }
            return Result.Ok();
        }

        public async Task LoadAsync(ItemDetailState state)
        {
            State = state;

            CatalogTypes = await DataProvider.GetCatalogTypesAsync();
            CatalogBrands = await DataProvider.GetCatalogBrandsAsync();

            int typeId = 0;

            if (state.Item != null)
            {
                var item = await DataProvider.GetItemByIdAsync(state.Item.Id);
                if (item == null)
                {
                    item = state.Item ?? CatalogItemModel.Empty;
                    item.CatalogType = item.CatalogType ?? new CatalogTypeModel();
                    IsUnavailable = true;
                }
                typeId = item.CatalogType.Id;
                Item = item;
            }
            else
            {
                Item = new CatalogItemModel();
            }
            var relatedItems = await DataProvider.GetItemsAsync(typeId, -1, null);
            var relatedItemsSkipCurrent = relatedItems.Where(r => r.Id != Item.Id);
            RelatedItems = new ObservableCollection<CatalogItemModel>(relatedItemsSkipCurrent);
        }

        public Task UnloadAsync()
        {
            Item = null;
            RelatedItems = null;
            return Task.CompletedTask;
        }

        public void OnClick(object sender, RoutedEventArgs e)
        {
            ShellViewModel.GoBack();
        }
    }
}
