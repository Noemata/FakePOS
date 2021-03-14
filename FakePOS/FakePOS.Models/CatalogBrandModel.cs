using System;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace FakePOS.Models
{
    public class CatalogBrand
    {
        public int Id { get; set; }
        public string Brand { get; set; }
    }

    public class CatalogBrandModel : ObservableObject
    {
        public CatalogBrandModel()
        {
            Id = 0;
            Name = "";
        }
        public CatalogBrandModel(CatalogBrand source)
        {
            Id = source.Id;
            Name = source.Brand;
        }

        public int Id { get; set; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is CatalogBrandModel instance)
            {
                return instance.Id == Id;
            }
            return false;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
