using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using CitySouth.Data.Models;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;
using System.Web.Security;

namespace CitySouth.Web.Controllers
{
    public class BaseController : ApiController
    {
        protected CitySouthContext db;
        protected sysUser user;
        protected Dictionary<string, object> result;
        protected List<string> message;
        protected List<string> GetErrorMessage(ICollection<ModelState> models)
        {
            List<string> message = new List<string>();
            foreach (var item in models)
            {
                foreach (var error in item.Errors)
                {
                    if (!string.IsNullOrEmpty(error.ErrorMessage))
                        message.Add(error.ErrorMessage);
                }
            }
            return message;
        }
        public BaseController()
        {
            result = new Dictionary<string, object>();
            result["code"] = "success";
            message = new List<string>();
            db = new CitySouthContext();
        }
        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            var authorization = controllerContext.Request.Headers.Authorization;
            if (authorization != null && !string.IsNullOrEmpty(authorization.Parameter))
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authorization.Parameter);
                user = db.sysUsers.FirstOrDefault(w => w.LoginName == ticket.Name);
                user.EstateIds = (from b in db.sysUserInEstates where b.UserId == user.UserId select b.EstateId).ToArray();
            }
            base.Initialize(controllerContext);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
