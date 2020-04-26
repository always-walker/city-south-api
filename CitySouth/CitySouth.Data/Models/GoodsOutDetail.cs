using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class GoodsOutDetail
    {
        public int DetailId { get; set; }
        public int OutId { get; set; }
        public int GoodsId { get; set; }
        public int Count { get; set; }
    }
}
