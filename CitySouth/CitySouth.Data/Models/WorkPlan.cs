using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class WorkPlan
    {
        public int PlanId { get; set; }
        public int EmployeeId { get; set; }
        public System.DateTime WorkDate { get; set; }
        public bool IsWork { get; set; }
    }
}
