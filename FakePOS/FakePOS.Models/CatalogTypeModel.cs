using System;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace FakePOS.Models
{
    public class CatalogType
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }

    public class CatalogTypeModel : ObservableObject
    {
        public CatalogTypeModel()
        {
            Id = 0;
            Name = "";
        }
        public CatalogTypeModel(CatalogType source)
        {
            Id = source.Id;
            Name = source.Type;
        }

        public int Id { get; set; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string ImageUrl
        {
            get
            {
                int id = Id <= 4 ? Id : 0;
                return $"ms-appx:///Assets/CatalogTypes/CatalogType{id}.jpg";
            }
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is CatalogTypeModel instance)
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
