using System;
using System.Collections.Generic;

namespace CitySouth.Data.Models
{
    public partial class sysConfig
    {
        public int ConfigId { get; set; }
        public int ParentConfigId { get; set; }
        public string ConfigKey { get; set; }
        public string ConfigKeyKeyName { get; set; }
        public string ConfigValue { get; set; }
        public string DataType { get; set; }
        public string CheckType { get; set; }
        public string GroupName { get; set; }
        public string Remark { get; set; }
        public string ErrorMsg { get; set; }
        public bool Ignore { get; set; }
        public int Sort { get; set; }
    }
}
