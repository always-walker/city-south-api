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
    public class PostController : BaseController
    {
        [HttpGet]
        [Author("post.list")]
        public Dictionary<string, object> Index()
        {
            var list = from b in db.Posts select b;
            result["datalist"] = list.ToList();
            return result;
        }
        [HttpPost]
        [Author("post.manage")]
        public Dictionary<string, object> add([FromBody]Post post)
        {
            if (string.IsNullOrEmpty(post.PostType))
            {
                result["code"] = "failed";
                message.Add("类型必选");
            }
            else if (string.IsNullOrEmpty(post.PostName))
            {
                result["code"] = "failed";
                message.Add("名称必填");
            }
            if (result["code"].ToString() == "success")
            {
                db.Posts.Add(post);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpPut]
        [Author("post.manage")]
        public Dictionary<string, object> modify([FromBody]Post post)
        {
            if (string.IsNullOrEmpty(post.PostType))
            {
                result["code"] = "failed";
                message.Add("类型必选");
            }
            else if (string.IsNullOrEmpty(post.PostName))
            {
                result["code"] = "failed";
                message.Add("名称必填");
            }
            if (result["code"].ToString() == "success")
            {
                Post newPost = db.Posts.FirstOrDefault(w => w.PostId == post.PostId);
                Ricky.ObjectCopy.Copy<Post>(post, newPost);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpDelete]
        [Author("post.delete")]
        public Dictionary<string, object> delete(int id)
        {
            if (db.Posts.Count(w => w.ParentPostId == id) > 0)
            {
                result["code"] = "failed";
                message.Add("请先删除下级岗位");
            }
            if (result["code"].ToString() == "success")
            {
                Post post = db.Posts.FirstOrDefault(w => w.PostId == id);
                db.Posts.Remove(post);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
    }
}
