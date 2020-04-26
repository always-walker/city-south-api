using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class WorkPlanTime
    {
        public int PlanId { get; set; }
        public int ConfigId { get; set; }
        public bool IsWork { get; set; }
    }
}
