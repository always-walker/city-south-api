using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CitySouth.Data.Models
{
    public partial class Owner
    {
        [NotMapped]
        public List<OwnerFamily> FamilyList { get; set; }
        [NotMapped]
        public List<OwnerCar> CarList { get; set; }
        [NotMapped]
        public string ExpireModifyRemark { get; set; }
    }
}
