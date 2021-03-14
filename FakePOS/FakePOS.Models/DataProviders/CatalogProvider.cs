using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FakePOS.Common;
using FakePOS.Models;

namespace FakePOS.Providers
{
    public partial class CatalogProvider : ICatalogProvider
    {
        static public async Task<Result> IsCurrentProviderAvailableAsync()
        {
            var provider = new CatalogProvider();
            return await provider.IsAvailableAsync();
        }

        public CatalogProvider()
        {
            LocalCatalogProvider = new LocalCatalogProvider();
        }

        public string Name => Current.Name;

        public ICatalogProvider LocalCatalogProvider { get; }

        public ICatalogProvider Current
        {
            get
            {
               return LocalCatalogProvider;
            }
        }

        public async Task<Result> IsAvailableAsync()
        {
            return await Current.IsAvailableAsync();
        }

        public async Task<IList<CatalogTypeModel>> GetCatalogTypesAsync()
        {
            try
            {
                CatalogTypes = await Current.GetCatalogTypesAsync();
                return CatalogTypes;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return new List<CatalogTypeModel>();
            }
        }

        public async Task<IList<CatalogBrandModel>> GetCatalogBrandsAsync()
        {
            try
            {
                CatalogBrands = await Current.GetCatalogBrandsAsync();
                return CatalogBrands;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return new List<CatalogBrandModel>();
            }
        }

        public async Task<CatalogItemModel> GetItemByIdAsync(int id)
        {
            try
            {
                return await Current.GetItemByIdAsync(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<IList<CatalogItemModel>> GetItemsAsync(int typeId, int brandId, string query)
        {
            try
            {
                return await Current.GetItemsAsync(typeId, brandId, query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return new List<CatalogItemModel>();
            }
        }

        public async Task<IList<CatalogItemModel>> GetItemsByVoiceCommandAsync(string query)
        {
            try
            {
                return await Current.GetItemsByVoiceCommandAsync(query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return new List<CatalogItemModel>();
            }
        }

        public async Task<IList<CatalogItemModel>> RelatedItemsByTypeAsync(int catalogTypeId)
        {
            try
            {
                return await Current.RelatedItemsByTypeAsync(catalogTypeId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return new List<CatalogItemModel>();
            }
        }

        public async Task SaveItemAsync(CatalogItemModel item)
        {
            await Current.SaveItemAsync(item);
        }

        public async Task DeleteItemAsync(CatalogItemModel catalogItem)
        {
            await Current.DeleteItemAsync(catalogItem);
        }
    }
}
