using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CitySouth.Data.Models
{
    public partial class sysRole
    {
        [NotMapped]
        public int[] AuthorIds { get; set; }
        [NotMapped]
        public bool Checked { get; set; }
    }
}
