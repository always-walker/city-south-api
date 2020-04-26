using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class WorkTimeConfig
    {
        public int ConfigId { get; set; }
        public System.TimeSpan TimeStart { get; set; }
        public System.TimeSpan TimeEnd { get; set; }
        public bool IsAble { get; set; }
    }
}
