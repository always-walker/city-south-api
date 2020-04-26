using CitySouth.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CitySouth.Web.Controllers
{
    public class RoleController : BaseController
    {
        [HttpGet]
        [Author("role.list")]
        public Dictionary<string, object> Index()
        {
            result["datalist"] = db.sysRoles.OrderByDescending(w => w.RoleId).ToList();
            return result;
        }
        private List<sysAuthor> GetAuthors(int ParentAuthorId, List<sysAuthorInRole> authors)
        {
            List<sysAuthor> list = db.sysAuthors.Where(m => m.ParentAuthorId == ParentAuthorId).OrderBy(m => m.Sort).ToList();
            foreach (sysAuthor author in list)
            {
                if (authors.Count(w => w.AuthorId == author.AuthorId) > 0)
                    author.Checked = true;
                if (db.sysAuthors.Count(m => m.ParentAuthorId == author.AuthorId) > 0)
                    author.children = GetAuthors(author.AuthorId, authors);
                else
                    author.children = new List<sysAuthor>();
            }
            return list;
        }
        [HttpGet]
        [Author("role.list")]
        public Dictionary<string, object> author(int id)
        {
            List<sysAuthorInRole> roleAuthors = db.sysAuthorInRoles.Where(w => w.RoleId == id).ToList();
            result["authors"] = GetAuthors(0, roleAuthors);
            return result;
        }
        [HttpPost]
        [Author("role.manage")]
        public Dictionary<string, object> add([FromBody]sysRole role)
        {
            if (string.IsNullOrEmpty(role.RoleName))
            {
                result["code"] = "failed";
                message.Add("角色名称必填");
            }
            if (result["code"].ToString() == "success")
            {
                db.sysRoles.Add(role);
                db.SaveChanges();
                foreach (int AuthorId in role.AuthorIds) {
                    sysAuthorInRole authorRole = new sysAuthorInRole();
                    authorRole.RoleId = role.RoleId;
                    authorRole.AuthorId = AuthorId;
                    db.sysAuthorInRoles.Add(authorRole);
                }
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpPut]
        [Author("role.manage")]
        public Dictionary<string, object> modify([FromBody]sysRole role)
        {
            if (string.IsNullOrEmpty(role.RoleName))
            {
                result["code"] = "failed";
                message.Add("角色名称必填");
            }
            if (result["code"].ToString() == "success")
            {
                sysRole newRole = db.sysRoles.FirstOrDefault(w => w.RoleId == role.RoleId);
                Ricky.ObjectCopy.Copy<sysRole>(role, newRole);
                var a = from b in db.sysAuthorInRoles where b.RoleId == role.RoleId select b;
                db.sysAuthorInRoles.RemoveRange(a);
                db.SaveChanges();
                foreach (int AuthorId in role.AuthorIds)
                {
                    sysAuthorInRole authorRole = new sysAuthorInRole();
                    authorRole.RoleId = role.RoleId;
                    authorRole.AuthorId = AuthorId;
                    db.sysAuthorInRoles.Add(authorRole);
                }
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpDelete]
        [Author("role.manage")]
        public Dictionary<string, object> delete(int id)
        {
            if (db.sysUserInRoles.Count(w => w.RoleId == id) > 0)
            {
                result["code"] = "failed";
                message.Add("必须先删除此角色的用户");
            }
            if (result["code"].ToString() == "success")
            {
                sysRole role = db.sysRoles.FirstOrDefault(w => w.RoleId == id);
                db.sysRoles.Remove(role);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
    }
}
