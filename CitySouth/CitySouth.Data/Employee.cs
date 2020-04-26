using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CitySouth.Data.Models
{
    public partial class Employee
    {
         [NotMapped]
        public string ChangePostRemark { get; set; }
         [NotMapped]
         public DateTime? ChangePostDate { get; set; }
    }
}
