using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CitySouth.Data.Models;
using System.Web.Security;
using System.Web;

namespace CitySouth.Web.Controllers
{
    public class ValuesController : BaseController
    {
        [AllowAnonymous]
        // GET api/values
        public string Get()
        {
            string strUser = "admin";
            string strPwd = "123456";
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(0, strUser, DateTime.Now,
                            DateTime.Now.AddSeconds(30), true, string.Format("{0}&{1}", strUser, strPwd),
                            FormsAuthentication.FormsCookiePath);
            string ticketStr = FormsAuthentication.Encrypt(ticket); 
            return ticketStr;
        }
        [HttpPost]
        [Author("xx.xx")]
        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public int xxx(int id)
        {
            return id;
        }
    }
}