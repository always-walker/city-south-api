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
                list.Add("���������˻�����");
            else if (IsVerfyPassword && string.IsNullOrEmpty(Password))
                list.Add("���������˻�����");
            else if (IsVerfyPassword && !Regex.IsMatch(Password, @"^[\S]{4,40}$"))
                list.Add("�˻�������������4λ");
            else if(IsVerfyPassword && db.sysUsers.Count(w=>w.LoginName == LoginName) > 0)
                list.Add("�˻������Ѵ���");
 
        }
        [NotMapped]
        public int[] RoleIds { get; set; }
        [NotMapped]
        public int[] EstateIds { get; set; }
    }
}
