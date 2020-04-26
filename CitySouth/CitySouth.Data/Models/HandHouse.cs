using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class HandHouse
    {
        public int HandId { get; set; }
        public int OwnerId { get; set; }
        public System.DateTime HandDate { get; set; }
        public System.DateTime CreateTime { get; set; }
        public Nullable<System.DateTime> PropertyStartDate { get; set; }
        public Nullable<int> UserId { get; set; }
        public string Remark { get; set; }
    }
}
