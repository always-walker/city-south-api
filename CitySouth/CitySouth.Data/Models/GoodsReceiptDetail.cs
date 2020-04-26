using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class GoodsReceiptDetail
    {
        public int DetailId { get; set; }
        public int ReceiptId { get; set; }
        public int GoodsId { get; set; }
        public int Count { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
