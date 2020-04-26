using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitySouth.Data
{
    public class UserPassword
    {
        [Required(ErrorMessage = "请输入旧密码")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "请输入您的新密码")]
        [RegularExpression(@"^[\S]{4,40}$", ErrorMessage = "密码至少输入4位.")]
        public string Password { get; set; }
    }
}
