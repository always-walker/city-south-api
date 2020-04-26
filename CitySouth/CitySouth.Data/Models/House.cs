using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class House
    {
        public int HouseId { get; set; }
        public int EstateId { get; set; }
        public string HouseType { get; set; }
        public int Building { get; set; }
        public int Unit { get; set; }
        public int Floor { get; set; }
        public int No { get; set; }
        public string HouseNo { get; set; }
        public string Model { get; set; }
        public string Structure { get; set; }
        public double Floorage { get; set; }
        public string ContactTel { get; set; }
        public string ElseTel { get; set; }
        public string News { get; set; }
        public bool IsPlace { get; set; }
        public bool IsHasOwner { get; set; }
        public Nullable<System.DateTime> HandDate { get; set; }
        public bool EmptyState { get; set; }
        public string Remark { get; set; }
    }
}
