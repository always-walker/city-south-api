using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CitySouth.Data.Models;
using CitySouth.Data;

namespace CitySouth.Web.Controllers
{
    public class UserController : BaseController
    {
        [HttpGet]
        [Author("user.list")]
        public Dictionary<string, object> Index()
        {
            result["datalist"] = db.sysUsers.OrderByDescending(w => w.CreateTime).ToList();
            return result;
        }
        [HttpPost]
        [Author("user.manage")]
        public Dictionary<string, object> add([FromBody] sysUser user)
        {
            user.Valid(message, db);
            if (message.Count == 0)
            {
                user.Password = Ricky.Common.EncryptString(user.Password);
                user.CreateTime = Ricky.Common.NowDate;
                db.sysUsers.Add(user);
                db.SaveChanges();
                foreach (int RoleId in user.RoleIds)
                {
                    sysUserInRole userRole = new sysUserInRole();
                    userRole.RoleId = RoleId;
                    userRole.UserId = user.UserId;
                    db.sysUserInRoles.Add(userRole);
                }
                foreach (int EstateId in user.EstateIds)
                {
                    sysUserInEstate userEstate = new sysUserInEstate();
                    userEstate.EstateId = EstateId;
                    userEstate.UserId = user.UserId;
                    db.sysUserInEstates.Add(userEstate);
                }
                db.SaveChanges();
            }
            else
            {
                result["code"] = "failed";
            }
            result["message"] = message;
            return result;
        }
        [HttpPut]
        [Author("user.manage")]
        public Dictionary<string, object> modify([FromBody] sysUser user)
        {
            user.Valid(message, db, false);
            if (message.Count == 0)
            {
                sysUser newUser = db.sysUsers.FirstOrDefault(w => w.UserId == user.UserId);
                if (!user.LoginName.Equals(newUser.LoginName) && db.sysUsers.Count(w => w.LoginName == user.LoginName) > 0)
                    message.Add("账户名称已存在");
                else
                {
                    Ricky.ObjectCopy.Copy<sysUser>(user, newUser, new string[] { "Password", "CreateTime", "IsSuper", "LastLoginIp", "LastLoginTime", "Status" });
                    if (!string.IsNullOrEmpty(user.Password))
                        newUser.Password = Ricky.Common.EncryptString(user.Password);
                    var a = from b in db.sysUserInRoles where b.UserId == user.UserId select b;
                    db.sysUserInRoles.RemoveRange(a);
                    var userOldEsates = from b in db.sysUserInEstates where b.UserId == user.UserId select b;
                    db.sysUserInEstates.RemoveRange(userOldEsates);
                    db.SaveChanges();
                    foreach (int RoleId in user.RoleIds)
                    {
                        sysUserInRole userRole = new sysUserInRole();
                        userRole.RoleId = RoleId;
                        userRole.UserId = user.UserId;
                        db.sysUserInRoles.Add(userRole);
                    }
                    foreach (int EstateId in user.EstateIds)
                    {
                        sysUserInEstate userEstate = new sysUserInEstate();
                        userEstate.EstateId = EstateId;
                        userEstate.UserId = user.UserId;
                        db.sysUserInEstates.Add(userEstate);
                    }
                    db.SaveChanges();
                }
            }
            else
            {
                result["code"] = "failed";
            }
            result["message"] = message;
            return result;
        }
        private List<Menus> GetMenus(int ParentAuthorId, List<int> authors)
        {
            List<sysAuthor> list = null;
            if (user.IsSuper)
            {
                list = db.sysAuthors.Where(m => m.ParentAuthorId == ParentAuthorId && !string.IsNullOrEmpty(m.MenuLink)).OrderBy(m => m.Sort).ToList();
            }
            else
            {
                list = db.sysAuthors.Where(m => m.ParentAuthorId == ParentAuthorId && authors.Contains(m.AuthorId)
                 && !string.IsNullOrEmpty(m.MenuLink)).OrderBy(m => m.Sort).ToList();
            }
            List<Menus> menus = new List<Menus>();
            foreach (sysAuthor autor in list)
            {
                Menus menu = new Menus(autor.AuthorName, autor.MenuLink, autor.MenuIcon);
                if (db.sysAuthors.Count(m => m.ParentAuthorId == autor.AuthorId) > 0)
                    menu.children = GetMenus(autor.AuthorId, authors);
                else
                    menu.children = new List<Menus>();
                menus.Add(menu);
            }
            return menus;
        }
        [HttpGet]
        public Dictionary<string, object> Menus()
        {
            List<int> authors = (from x in db.sysAuthorInRoles
                                 join y in db.sysRoles
                                     on x.RoleId equals y.RoleId
                                 join z in db.sysUserInRoles on y.RoleId equals z.RoleId
                                 where z.UserId == user.UserId
                                 select x.AuthorId).ToList();
            result["menus"] = GetMenus(0, authors);
            if (user.IsSuper)
                result["authors"] = (from b in db.sysAuthors where !string.IsNullOrEmpty(b.AuthorKey) select b.AuthorKey).ToArray();
            else
                result["authors"] = (from b in db.sysAuthors where authors.Contains(b.AuthorId) && !string.IsNullOrEmpty(b.AuthorKey) select b.AuthorKey).ToArray();
            return result;
        }
        [HttpDelete]
        [Author("user.manage")]
        public Dictionary<string, object> delete(int id)
        {
            if (db.sysUserLogs.Count(w => w.UserId == id) > 0)
            {
                result["code"] = "failed";
                message.Add("此用户已经存在系统操作，不可删除。");
            }
            if (result["code"].ToString() == "success")
            {
                sysUser user = db.sysUsers.FirstOrDefault(w => w.UserId == id);
                if (user.IsSuper)
                {
                    result["code"] = "failed";
                    message.Add("此用户为系统用户，不可删除。");
                }
                else
                {
                    db.sysUsers.Remove(user);
                    db.SaveChanges();
                }
            }
            result["message"] = message;
            return result;
        }
        [HttpGet]
        public Dictionary<string, object> roles(int id)
        {
            List<sysUserInRole> myroles = db.sysUserInRoles.Where(w => w.UserId == id).ToList();
            List<sysRole> roles = db.sysRoles.OrderBy(w => w.RoleId).ToList();
            foreach (sysRole role in roles)
            {
                if (myroles.Count(w => w.RoleId == role.RoleId) > 0)
                    role.Checked = true;
            }
            result["roles"] = roles;
            return result;
        }
        [HttpGet]
        public Dictionary<string, object> estates(int id)
        {
            List<sysUserInEstate> myEstates = db.sysUserInEstates.Where(w => w.UserId == id).ToList();
            List<Estate> estates = db.Estates.OrderBy(w => w.EstateId).ToList();
            foreach (Estate estate in estates)
            {
                if (myEstates.Count(w => w.EstateId == estate.EstateId) > 0)
                    estate.Checked = true;
            }
            result["estates"] = estates;
            return result;
        }
        [HttpPost]
        public Dictionary<string, object> changePassword([FromBody]UserPassword pass)
        {
            if (ModelState.IsValid)
            {
                if(user.Password.Equals(Ricky.Common.EncryptString(pass.OldPassword)))
                {
                    user.Password = Ricky.Common.EncryptString(pass.Password);
                    db.SaveChanges();
                }
                else
                {
                    result["code"] = "failed";
                    message.Add("旧密码输入错误。");
                }
            }
            else
            {
                result["code"] = "failed";
                message = GetErrorMessage(ModelState.Values);
            }
            result["message"] = message;
            return result;
        } 
    }
}
