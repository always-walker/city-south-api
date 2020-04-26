using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CitySouth.Data.Models;

namespace CitySouth.Web.Controllers
{
    public class AuthorController : BaseController
    {
        private List<sysAuthor> GetAuthors(int ParentAuthorId)
        {
            List<sysAuthor> list = db.sysAuthors.Where(m => m.ParentAuthorId == ParentAuthorId).OrderBy(m => m.Sort).ToList();
            foreach (sysAuthor autor in list)
            {
                if (db.sysAuthors.Count(m => m.ParentAuthorId == autor.AuthorId) > 0)
                    autor.children = GetAuthors(autor.AuthorId);
                else
                    autor.children = new List<sysAuthor>();
            }
            return list;
        }
        [HttpGet]
        [Author("author.list")]
        public Dictionary<string, object> Index()
        {
            result["datalist"] = GetAuthors(0);
            return result;
        }
        [HttpPost]
        [Author("author.manage")]
        public Dictionary<string, object> add([FromBody]sysAuthor author)
        {
            if(string.IsNullOrEmpty(author.AuthorName))
            {
                result["code"] = "failed";
                message.Add("资源名称必填");
            }
            if (result["code"].ToString() == "success")
            {
                db.sysAuthors.Add(author);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpPut]
        [Author("author.manage")]
        public Dictionary<string, object> modify([FromBody]sysAuthor author)
        {
            if (string.IsNullOrEmpty(author.AuthorName))
            {
                result["code"] = "failed";
                message.Add("资源名称必填");
            }
            if (result["code"].ToString() == "success")
            {
                sysAuthor newAuthor = db.sysAuthors.FirstOrDefault(w => w.AuthorId == author.AuthorId);
                Ricky.ObjectCopy.Copy<sysAuthor>(author, newAuthor);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpDelete]
        [Author("author.manage")]
        public Dictionary<string, object> delete(int id)
        {
            if(db.sysAuthors.Count(w=>w.ParentAuthorId == id) > 0)
            {
                result["code"] = "failed";
                message.Add("必须先删除子资源");
            }
            if (result["code"].ToString() == "success")
            {
                sysAuthor author = db.sysAuthors.FirstOrDefault(w => w.AuthorId == id);
                db.sysAuthors.Remove(author);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
    }
}
