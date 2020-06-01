using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class ElseCost
    {
        public int ElseCostId { get; set; }
        public string CostName { get; set; }
        public int OwnerId { get; set; }
        public string PayerName { get; set; }
        public Nullable<int> ConfigId { get; set; }
        public decimal UnitPrice { get; set; }
        public bool IsDeposit { get; set; }
        public decimal Amount { get; set; }
        public decimal LeftAmount { get; set; }
        public System.DateTime CreateTime { get; set; }
        public Nullable<System.DateTime> PayTime { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string ReceiptNo { get; set; }
        public string VoucherNo { get; set; }
        public Nullable<int> UserId { get; set; }
        public string OprationName { get; set; }
        public string PayWay { get; set; }
        public string Remark { get; set; }
        public Nullable<int> Status { get; set; }
    }
}
