using System;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using FakePOS.Providers;

namespace FakePOS.Models
{
    public class CatalogItem
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }

        public string PictureFileName { get; set; }
        public string PictureUri { get; set; }

        public int CatalogTypeId { get; set; }
        public int CatalogBrandId { get; set; }

        public bool IsDisabled { get; set; }

        public DateTime LatUpdate { get; set; }
    }

    public partial class CatalogItemModel : ObservableObject
    {
        static public readonly CatalogItemModel Empty = new CatalogItemModel();

        public CatalogItemModel(int id = 0) : this(new CatalogItem { Id = id })
        {
        }
        public CatalogItemModel(CatalogItem source)
        {
            Source = source;
            CopyValues(Source);
        }

        public CatalogItem Source { get; private set; }

        public int Id { get => Source.Id; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private double _price;
        public double Price
        {
            get { return _price; }
            set { SetProperty(ref _price, value); OnPropertyChanged("PriceDesc"); UpdateDiscount(); }
        }

        public string PriceDesc => $"${Price.ToString("0.00")}";

        public string PriceString
        {
            get { return Price.ToString("0.00"); }
            set { Price = ParseDecimal(value); OnPropertyChanged("PriceString"); }
        }

        private double ParseDecimal(string value)
        {
            double d = 0;
            Double.TryParse(value, out d);
            return d;
        }

        public string PictureFileName { get; set; }
        public byte[] Picture { get; set; }
        public string PictureContentType { get; set; }

        private string _pictureUri;
        public string PictureUri
        {
            get { return _pictureUri ?? " "; }
            set { SetProperty(ref _pictureUri, value); }
        }

        private CatalogTypeModel _catalogType;
        public CatalogTypeModel CatalogType
        {
            get { return _catalogType; }
            set { SetProperty(ref _catalogType, value); }
        }

        private CatalogBrandModel _catalogBrand;
        public CatalogBrandModel CatalogBrand
        {
            get { return _catalogBrand; }
            set { SetProperty(ref _catalogBrand, value); }
        }

        private bool _isDisabled;
        public bool IsDisabled
        {
            get { return _isDisabled; }
            set { SetProperty(ref _isDisabled, value); }
        }

        // Management Properties

        public bool IsSelected { get; set; }

        private bool _isNew = false;
        public bool IsNew
        {
            get { return _isNew; }
            set { SetProperty(ref _isNew, value); }
        }

        private bool _isDeleted = false;
        public bool IsDeleted
        {
            get { return _isDeleted; }
            set { SetProperty(ref _isDeleted, value); }
        }

        public bool HasChanges
        {
            get
            {
                return
                    Source.Name != Name ||
                    Source.Description != Description ||
                    Source.Price != Price ||
                    Source.PictureUri != PictureUri ||
                    Source.CatalogTypeId != CatalogType.Id ||
                    Source.CatalogBrandId != CatalogBrand.Id ||
                    Source.IsDisabled != IsDisabled;
            }
        }

        public void Undo()
        {
            CopyValues(Source);

            IsDeleted = false;
            OnPropertyChanged(nameof(HasChanges));
        }

        private void CopyValues(CatalogItem source)
        {
            Name = source.Name;
            Description = source.Description;
            Price = source.Price;
            PictureUri = source.PictureUri;
            CatalogType = CatalogProvider.GetCatalogType(source.CatalogTypeId) ?? new CatalogTypeModel();
            CatalogBrand = CatalogProvider.GetCatalogBrand(source.CatalogBrandId) ?? new CatalogBrandModel();
            IsDisabled = source.IsDisabled;

            IsMultipleSizes = true;
            IsSizeS = true;
            IsSizeM = true;
            IsSizeL = true;
        }

        public void Commit()
        {
            Source.Name = Name;
            Source.Description = Description;
            Source.Price = Price;
            Source.PictureUri = PictureUri;
            Source.CatalogTypeId = CatalogType.Id;
            Source.CatalogBrandId = CatalogBrand.Id;
            Source.IsDisabled = IsDisabled;

            OnPropertyChanged(nameof(HasChanges));
        }

        public override bool Equals(object obj)
        {
            var model = obj as CatalogItemModel;
            if (model != null)
            {
                return model.Id == Id;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
