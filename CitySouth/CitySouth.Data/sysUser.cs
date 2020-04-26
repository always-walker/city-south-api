using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using System.Linq;

namespace CitySouth.Data.Models
{
    public partial class sysUser
    {
        public void Valid(List<string> list, CitySouthContext db, bool IsVerfyPassword = true)
        {
            if (string.IsNullOrEmpty(LoginName))
                list.Add("必须输入账户名称");
            else if (IsVerfyPassword && string.IsNullOrEmpty(Password))
                list.Add("必须输入账户密码");
            else if (IsVerfyPassword && !Regex.IsMatch(Password, @"^[\S]{4,40}$"))
                list.Add("账户密码至少输入4位");
            else if(IsVerfyPassword && db.sysUsers.Count(w=>w.LoginName == LoginName) > 0)
                list.Add("账户名称已存在");
 
        }
        [NotMapped]
        public int[] RoleIds { get; set; }
        [NotMapped]
        public int[] EstateIds { get; set; }
    }
}
