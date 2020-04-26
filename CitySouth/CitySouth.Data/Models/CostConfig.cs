using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class CostConfig
    {
        public int ConfigId { get; set; }
        public int EstateId { get; set; }
        public string ConfigType { get; set; }
        public string ConfigVersion { get; set; }
        public string Category { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public string UnitName { get; set; }
        public Nullable<int> UserId { get; set; }
        public System.DateTime ModifyDate { get; set; }
        public bool IsDefault { get; set; }
        public bool IsAble { get; set; }
        public string DiscountMode { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> Amount { get; set; }
        public string Remark { get; set; }
    }
}
