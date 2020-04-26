using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class GoodsCategory
    {
        public int CategoryId { get; set; }
        public int ParentCategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
