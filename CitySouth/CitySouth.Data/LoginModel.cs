using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitySouth.Data
{
    public class LoginModel
    {
        [Required(ErrorMessage = "请输入您的账户")]
        public string name { get; set; }
        [Required(ErrorMessage = "请输入您的密码")]
        [RegularExpression(@"^[\S]{4,40}$", ErrorMessage = "密码至少输入4位.")]
        public string password { get; set; }
    }
}
