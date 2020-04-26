using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class Good
    {
        public int GoodsId { get; set; }
        public int CategoryId { get; set; }
        public string GoodsNo { get; set; }
        public string GoodsImage { get; set; }
        public string GoodsName { get; set; }
        public string Model { get; set; }
        public string UnitName { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
