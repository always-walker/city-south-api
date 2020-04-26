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
    public class CategoryController : BaseController
    {
        [HttpGet]
        [Author("goods-category.list")]
        public Dictionary<string, object> Index()
        {
            var list = from b in db.GoodsCategories select b;
            result["datalist"] = list.ToList();
            return result;
        }
        [HttpPost]
        [Author("goods-category.manage")]
        public Dictionary<string, object> add([FromBody]GoodsCategory category)
        {
            if (string.IsNullOrEmpty(category.CategoryName))
            {
                result["code"] = "failed";
                message.Add("分类名称必填");
            }
            if (result["code"].ToString() == "success")
            {
                db.GoodsCategories.Add(category);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpPut]
        [Author("goods-category.manage")]
        public Dictionary<string, object> modify([FromBody]GoodsCategory category)
        {
            if (string.IsNullOrEmpty(category.CategoryName))
            {
                result["code"] = "failed";
                message.Add("分类名称必填");
            }
            if (result["code"].ToString() == "success")
            {
                GoodsCategory newCategory = db.GoodsCategories.FirstOrDefault(w => w.CategoryId == category.CategoryId);
                Ricky.ObjectCopy.Copy<GoodsCategory>(category, newCategory);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpDelete]
        [Author("goods-category.delete")]
        public Dictionary<string, object> delete(int id)
        {
            if (db.GoodsCategories.Count(w => w.ParentCategoryId == id) > 0)
            {
                result["code"] = "failed";
                message.Add("请先删除下级分类");
            }
            else if (db.Goods.Count(w => w.CategoryId == id) > 0)
            {
                result["code"] = "failed";
                message.Add("请先删除分类下属产品");
            }
            if (result["code"].ToString() == "success")
            {
                GoodsCategory category = db.GoodsCategories.FirstOrDefault(w => w.CategoryId == id);
                db.GoodsCategories.Remove(category);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
    }
}
