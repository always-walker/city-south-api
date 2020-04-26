using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitySouth.Data
{
    public class MonthPlan
    {
        public MonthPlan(int employeeId, string employeeName, int postId, int estateId) 
        {
            this.EmployeeId = employeeId;
            this.EmployeeName = employeeName;
            this.PostId = postId;
            this.EstateId = estateId;
        }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int PostId { get; set; }
        public int EstateId { get; set; }
        public bool[] Works { get; set; }
        public bool[,] WorkTimes { get; set; }
    }
}
