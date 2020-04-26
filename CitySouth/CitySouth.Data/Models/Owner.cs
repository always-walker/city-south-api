using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class Owner
    {
        public int OwnerId { get; set; }
        public int HouseId { get; set; }
        public string CheckInType { get; set; }
        public string OwnerName { get; set; }
        public string CheckInName { get; set; }
        public string CardId { get; set; }
        public string Phone { get; set; }
        public string Occupation { get; set; }
        public Nullable<System.DateTime> PropertyStartDate { get; set; }
        public Nullable<System.DateTime> PropertyExpireDate { get; set; }
        public Nullable<System.DateTime> HandDate { get; set; }
        public Nullable<System.DateTime> RenovationDate { get; set; }
        public Nullable<System.DateTime> CheckInDate { get; set; }
        public string UseInfo { get; set; }
        public string Remark { get; set; }
    }
}
