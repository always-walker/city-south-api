using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CitySouth.Data.Models
{
    public partial class WorkPlan
    {
        [NotMapped]
        public int ConfigId { get; set; }
    }
}
