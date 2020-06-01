using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class ApplyLeave
    {
        public int LeaveId { get; set; }
        public int EmployeeId { get; set; }
        public string LeaveType { get; set; }
        public string Reason { get; set; }
        public System.DateTime CreateTime { get; set; }
        public double Days { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public Nullable<bool> IsPass { get; set; }
        public Nullable<int> PassEmployeeId { get; set; }
        public string Image { get; set; }
        public string Remark { get; set; }
        public bool IsDelete { get; set; }
    }
}
