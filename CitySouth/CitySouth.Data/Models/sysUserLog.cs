using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class sysUserLog
    {
        public int LogId { get; set; }
        public int UserId { get; set; }
        public string LoginName { get; set; }
        public string LogType { get; set; }
        public string Remark { get; set; }
        public string Ip { get; set; }
        public System.DateTime LogTime { get; set; }
    }
}
