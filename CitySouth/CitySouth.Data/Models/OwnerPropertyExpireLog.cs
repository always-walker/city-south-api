using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class OwnerPropertyExpireLog
    {
        public int LogId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int OwnerId { get; set; }
        public Nullable<System.DateTime> ExpireDate { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public string Remark { get; set; }
        public System.DateTime CreateTime { get; set; }
    }
}
