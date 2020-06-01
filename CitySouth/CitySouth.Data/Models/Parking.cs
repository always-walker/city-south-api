using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class Parking
    {
        public int ParkingId { get; set; }
        public int CarId { get; set; }
        public string PayerName { get; set; }
        public int ConfigId { get; set; }
        public decimal UnitPrice { get; set; }
        public double MonthCount { get; set; }
        public decimal Amount { get; set; }
        public System.DateTime CreateTime { get; set; }
        public Nullable<System.DateTime> PayTime { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public string ReceiptNo { get; set; }
        public string VoucherNo { get; set; }
        public Nullable<int> UserId { get; set; }
        public string OprationName { get; set; }
        public string PayWay { get; set; }
        public string Remark { get; set; }
        public int Status { get; set; }
    }
}
