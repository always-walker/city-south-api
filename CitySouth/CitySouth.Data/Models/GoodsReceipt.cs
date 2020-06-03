using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class GoodsReceipt
    {
        public int ReceiptId { get; set; }
        public string ReceiptNo { get; set; }
        public int EstateId { get; set; }
        public string Reason { get; set; }
        public string Remark { get; set; }
        public string Purchaser { get; set; }
        public string Submitter { get; set; }
        public System.DateTime SubmitDate { get; set; }
        public string StorageChecker { get; set; }
        public System.DateTime InStorageDate { get; set; }
        public bool IsDelete { get; set; }
    }
}
