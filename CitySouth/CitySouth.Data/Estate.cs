using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CitySouth.Data.Models
{
    public partial class Estate
    {
        [NotMapped]
        public bool Checked { get; set; }
    }
}
