using System;

namespace FakePOS.Models
{
    partial class CatalogItemModel
    {
        private bool _isDiscountEnabled;
        public bool IsDiscountEnabled
        {
            get { return _isDiscountEnabled; }
            set { SetProperty(ref _isDiscountEnabled, value); UpdateDiscount(); }
        }

        private double _discountPercent;
        public double DiscountPercent
        {
            get { return _discountPercent; }
            set { SetProperty(ref _discountPercent, value); UpdateDiscount(); }
        }

        public double DiscountValue => IsDiscountEnabled ? -Math.Round(Price * (DiscountPercent / 100.0), 2) : 0;

        public double FinalPrice => Price + DiscountValue;

        private void UpdateDiscount()
        {
            OnPropertyChanged(nameof(DiscountValue));
            OnPropertyChanged(nameof(FinalPrice));
        }


        private bool _isDiscountFromEnabled;
        public bool IsDiscountFromEnabled
        {
            get { return _isDiscountFromEnabled; }
            set { SetProperty(ref _isDiscountFromEnabled, value); UpdateDiscountDates(); }
        }

        private bool _isDiscountUntilEnabled;
        public bool IsDiscountUntilEnabled
        {
            get { return _isDiscountUntilEnabled; }
            set { SetProperty(ref _isDiscountUntilEnabled, value); UpdateDiscountDates(); }
        }

        private DateTimeOffset? _dateFrom;
        public DateTimeOffset? DateFrom
        {
            get { return _dateFrom; }
            set { SetProperty(ref _dateFrom, value); }
        }

        private DateTimeOffset? _dateUntil;
        public DateTimeOffset? DateUntil
        {
            get { return _dateUntil; }
            set { SetProperty(ref _dateUntil, value); }
        }

        private void UpdateDiscountDates()
        {
            DateFrom = IsDiscountFromEnabled ? DateFrom : null;
            DateUntil = IsDiscountUntilEnabled ? DateUntil : null;
        }
    }
}
