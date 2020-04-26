using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class sysUserInRole
    {
        public int UserInRoleId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}
