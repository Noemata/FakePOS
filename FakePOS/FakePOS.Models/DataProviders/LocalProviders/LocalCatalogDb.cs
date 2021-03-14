using System;
using System.IO;
using System.Collections.Generic;

using FakePOS.Models;
using FakePOS.Services;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

namespace FakePOS.Providers
{
    public class LocalCatalogDb : JsonDb
    {
        const string CURRENT_VERSION = "1.0";
        const string DEFAULT_FILENAME = "LocalCatalogDb.json";

        public LocalCatalogDb(string fileName = DEFAULT_FILENAME) : base(fileName)
        {
            // MP! note: DB load.
            if (!File.Exists(base.FilePath) || Version != CURRENT_VERSION)
            {
                IResourceService res = Ioc.Default.GetService<IResourceService>();
                string json = res.LoadString("CatalogDb.CatalogDb.json");
                Deserialize(json);
                Version = CURRENT_VERSION;
                SaveChanges();
            }
        }

        static public void ResetData(string fileName = DEFAULT_FILENAME)
        {
            File.Delete(GetFilePath(fileName));
        }

        public string Version { get; set; }

        public List<CatalogType> CatalogTypes { get; set; }
        public List<CatalogBrand> CatalogBrands { get; set; }
        public List<CatalogItem> CatalogItems { get; set; }
    }
}
