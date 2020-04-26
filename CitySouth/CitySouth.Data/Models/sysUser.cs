using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class sysUser
    {
        public int UserId { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string Sex { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string HeadUrl { get; set; }
        public bool IsSuper { get; set; }
        public System.DateTime CreateTime { get; set; }
        public Nullable<System.DateTime> LastLoginTime { get; set; }
        public string LastLoginIp { get; set; }
        public int Status { get; set; }
    }
}
