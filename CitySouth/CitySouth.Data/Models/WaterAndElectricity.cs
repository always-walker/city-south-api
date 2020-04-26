using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class WaterAndElectricity
    {
        public int FeeId { get; set; }
        public int OwnerId { get; set; }
        public string PayerName { get; set; }
        public int ConfigId { get; set; }
        public string FeeType { get; set; }
        public string FeeName { get; set; }
        public System.DateTime CreateTime { get; set; }
        public Nullable<System.DateTime> PayTime { get; set; }
        public Nullable<System.DateTime> FeeDate { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public string UnitName { get; set; }
        public Nullable<double> LastQuantity { get; set; }
        public Nullable<double> Quantity { get; set; }
        public decimal Amount { get; set; }
        public string ReceiptNo { get; set; }
        public string VoucherNo { get; set; }
        public Nullable<int> UserId { get; set; }
        public string OprationName { get; set; }
        public string PayWay { get; set; }
        public string Remark { get; set; }
        public int Status { get; set; }
    }
}
