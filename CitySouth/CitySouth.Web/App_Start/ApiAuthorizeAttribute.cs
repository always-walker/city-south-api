using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Security;

namespace CitySouth.Web
{
    public class AuthorAttribute : Attribute
    {
        public string Key { get; set; }
        public AuthorAttribute()
        {
        }
        public AuthorAttribute(string _key)
        {
            this.Key = _key;
        }
    }
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        private string code = "failed";
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            //从http请求的头里面获取身份验证信息，验证是否是请求发起方的ticket
            var authorization = actionContext.Request.Headers.Authorization;
            if (authorization != null && !string.IsNullOrEmpty(authorization.Parameter))
            {
                //var controllerAuthor = actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AuthorAttribute>().FirstOrDefault();
                var actionAuthor = actionContext.ActionDescriptor.GetCustomAttributes<AuthorAttribute>().FirstOrDefault();
                //解密用户ticket,并校验用户名密码是否匹配
                return ValidateTicket(authorization.Parameter, actionAuthor);
            }
            //如果取不到身份验证信息，并且不允许匿名访问，则返回未验证401
            else
            {
                //var attributes = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().OfType<AllowAnonymousAttribute>();
                //return attributes.Any(a => a is AllowAnonymousAttribute);
                this.code = "notlogin";
                return false;
            }
        }
        //校验用户名密码（正式环境中应该是数据库校验）
        private bool ValidateTicket(string encryptTicket, AuthorAttribute actionAuthor)
        {
            //解密Ticket
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(encryptTicket);
            if (ticket.Expired)
            {
                this.code = "notlogin";
                return false;
            }
            else
            {
                this.code = "noauthor";
                if (actionAuthor == null)
                {
                    return true;
                }
                else
                {
                    bool HasAuthor = false;
                    var name = FormsAuthentication.Decrypt(encryptTicket).Name;
                    using(CitySouth.Data.Models.CitySouthContext db = new Data.Models.CitySouthContext())
                    {
                        CitySouth.Data.Models.sysUser user = db.sysUsers.FirstOrDefault(w => w.LoginName == name);
                        if (user.IsSuper)
                        {
                            HasAuthor = true;
                        }
                        else
                        {
                            string[] keys = actionAuthor.Key.Split(',');
                            var authors = from u in db.sysUsers
                                          join ur in db.sysUserInRoles on u.UserId equals ur.UserId
                                          join ar in db.sysAuthorInRoles on ur.RoleId equals ar.RoleId
                                          join a in db.sysAuthors on ar.AuthorId equals a.AuthorId
                                          where u.LoginName == name && keys.Contains(a.AuthorKey)
                                          select a.AuthorKey;
                            if (authors.Count() > 0)
                                HasAuthor = true;
                        }
                        db.Database.Connection.Close();
                    }
                    return HasAuthor;
                }
            }
        }
        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            base.HandleUnauthorizedRequest(actionContext);
            var content = new
            {
                code = code,
                message = new string[] { "您没有权限执行此操作" }
            };
            actionContext.Response.Content = new StringContent(System.Web.Helpers.Json.Encode(content), Encoding.UTF8, "application/json");
        }
    }
}