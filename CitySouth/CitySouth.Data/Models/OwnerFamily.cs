using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class OwnerFamily
    {
        public int PeopleId { get; set; }
        public int OwnerId { get; set; }
        public string PeopleName { get; set; }
        public string Sex { get; set; }
        public string Relation { get; set; }
        public string Phone { get; set; }
        public string Remark { get; set; }
    }
}
