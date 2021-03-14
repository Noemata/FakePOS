using System;

using FakePOS.Common;
using FakePOS.Models;

namespace FakePOS.ViewModels
{
    public class ItemDetailState : ViewState
    {
        public ItemDetailState(int id = 0)
        {
            Item = new CatalogItemModel(id);
        }
        public ItemDetailState(CatalogItemModel item) : this()
        {
            Item = item;
        }

        public CatalogItemModel Item { get; set; }
    }
}
