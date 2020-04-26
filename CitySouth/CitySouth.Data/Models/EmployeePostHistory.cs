using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class EmployeePostHistory
    {
        public int HistoryId { get; set; }
        public int EmployeeId { get; set; }
        public int CurrentPostId { get; set; }
        public int ChangePostId { get; set; }
        public System.DateTime CreateTime { get; set; }
        public Nullable<System.DateTime> ChangeDate { get; set; }
        public int UserId { get; set; }
        public string Transactor { get; set; }
        public string Remark { get; set; }
    }
}
