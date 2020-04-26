using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class GoodsStorage
    {
        public int StorageId { get; set; }
        public int EstateId { get; set; }
        public int GoodsId { get; set; }
        public int Count { get; set; }
        public decimal Amount { get; set; }
    }
}
