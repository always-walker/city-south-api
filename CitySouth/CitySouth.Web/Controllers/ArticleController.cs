using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CitySouth.Data.Models;
using CitySouth.Data;
using Ricky;

namespace CitySouth.Web.Controllers
{
    public class ArticleController : BaseController
    {
        [HttpPost]
        [Author("article.list")]
        public Dictionary<string, object> Index([FromBody]SearchModel model)
        {
            var a = from b in db.Articles select b;
            if (!string.IsNullOrEmpty(model.FkIds))
            {
                model.FkIds = "," + model.FkIds + ",";
                a = a.Where(w => w.EstateIds.Contains(model.FkIds));
            }
            else if (!user.IsSuper)
            {
                model.FkIds = "," + string.Join(",", user.EstateIds) + ",";
                a = a.Where(w => w.EstateIds.Contains(model.FkIds));
            }
            if (!string.IsNullOrEmpty(model.KeyWord))
                a = a.Where(w => w.Title.Contains(model.KeyWord));
            result["count"] = a.Count();
            var list = a.OrderBy(w => w.Sort).ThenByDescending(w => w.AddTime).Skip((model.PageIndex - 1) * model.PageSize).Take(model.PageSize).ToList();
            result["datalist"] = list;
            return result;
        }
        [HttpPost]
        [Author("article.manage")]
        public Dictionary<string, object> Add([FromBody]Article article)
        {
            if (string.IsNullOrEmpty(article.Title))
            {
                result["code"] = "failed";
                message.Add("标题不能为空");
            }
            else
            {
                db.Articles.Add(article);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpPut]
        [Author("article.manage")]
        public Dictionary<string, object> Modify([FromBody]Article article)
        {
            if (string.IsNullOrEmpty(article.Title))
            {
                result["code"] = "failed";
                message.Add("标题不能为空");
            }
            else
            {
                Article newArticle = db.Articles.FirstOrDefault(w => w.ArticleId == article.ArticleId);
                Ricky.ObjectCopy.Copy<Article>(article, newArticle, new string[] { "Click", "AddTime" });
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpDelete]
        [Author("article.manage")]
        public Dictionary<string, object> Delete(int id)
        {
            Article article = db.Articles.FirstOrDefault(w => w.ArticleId == id);
            db.Articles.Remove(article);
            db.SaveChanges();
            result["message"] = message;
            return result;
        }
    }
}
