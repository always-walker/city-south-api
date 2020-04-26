using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CitySouth.Data.Models
{
    public partial class sysAuthor
    {
        [NotMapped]
        public List<sysAuthor> children { get; set; }
        [NotMapped]
        public bool Checked { get; set; }
    }
}
