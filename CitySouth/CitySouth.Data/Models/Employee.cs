using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class Employee
    {
        public int EmployeeId { get; set; }
        public string EmployeeNo { get; set; }
        public string EmployeeName { get; set; }
        public int EstateId { get; set; }
        public string Sex { get; set; }
        public string CardId { get; set; }
        public int PostId { get; set; }
        public string Education { get; set; }
        public System.DateTime EntryDate { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string UrgentContactName { get; set; }
        public string UrgentContactPhone { get; set; }
        public Nullable<System.DateTime> QuitDate { get; set; }
        public string QuitRemark { get; set; }
        public string Remark { get; set; }
    }
}
