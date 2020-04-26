using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CitySouth.Data;
using CitySouth.Data.Models;
using System.Web.Security;

namespace CitySouth.Web.Controllers
{
    public class LoginController : BaseController
    {
        [AllowAnonymous]
        [HttpPost]
        public Dictionary<string, object> Index([FromBody]LoginModel model)
        {
            if (ModelState.IsValid)
            {
                string password = Ricky.Common.EncryptString(model.password);
                sysUser user = db.sysUsers.FirstOrDefault(w => w.LoginName == model.name && w.Password == password);
                if (user != null)
                {
                    user.LastLoginTime = Ricky.Common.NowDate;
                    user.LastLoginIp = Request.RequestUri.Host;
                    db.SaveChanges();
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(0, model.name, DateTime.Now,
                            DateTime.Now.AddHours(12), true, string.Format("{0}&{1}", model.name, model.password),
                            FormsAuthentication.FormsCookiePath);
                    result["ticket"] = FormsAuthentication.Encrypt(ticket);
                }
                else
                {
                    result["code"] = "failed";
                    result["message"] = new List<string> { "用户名或密码错误" };
                }
            }
            else
            {
                result["code"] = "failed";
                result["message"] = GetErrorMessage(ModelState.Values);
            }
            return result;
        }
    }
}
