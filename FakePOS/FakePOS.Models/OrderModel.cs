using System;

namespace FakePOS.Models
{
    public class OrderModel
    {
        public DateTime OrderDate { get; set; }
        public int CatalogTypeId { get; set; }
        public double OrderTotal { get; set; }
    }
}
