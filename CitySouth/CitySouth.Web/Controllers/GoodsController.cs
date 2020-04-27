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
    public class GoodsController : BaseController
    {
        private void setCategoryIds(List<int> CategoryIds, List<GoodsCategory> categorys, int ParentCategoryId)
        {
            CategoryIds.Add(ParentCategoryId);
            List<GoodsCategory> childCategorys = categorys.Where(w => w.ParentCategoryId == ParentCategoryId).ToList();
            foreach (GoodsCategory category in childCategorys)
            {
                setCategoryIds(CategoryIds, categorys, category.CategoryId);
            }
        }
        [HttpPost]
        [Author("goods.list")]
        public Dictionary<string, object> Index([FromBody]SearchModel model)
        {
            List<int> CategoryIds = new List<int>();
            if (model.FkId != null && model.FkId > 0)
            {
                List<GoodsCategory> categorys = db.GoodsCategories.ToList();
                setCategoryIds(CategoryIds, categorys, model.FkId.Value);
            }
            var a = from b in db.Goods select b;
            if (CategoryIds.Count > 0)
                a = a.Where(w => CategoryIds.Contains(w.CategoryId));
            if (!string.IsNullOrEmpty(model.KeyWord))
                a = a.Where(w => w.GoodsName.Contains(model.KeyWord) || w.GoodsNo.Contains(model.KeyWord));
            result["count"] = a.Count();
            var list = a.ToList();
            result["datalist"] = list;
            return result;
        }
        [HttpPost]
        [Author("goods.manage")]
        public Dictionary<string, object> Add([FromBody]Good good)
        {
            if (string.IsNullOrEmpty(good.GoodsNo))
            {
                result["code"] = "failed";
                message.Add("物资编号不能为空");
            }
            else if (string.IsNullOrEmpty(good.GoodsName))
            {
                result["code"] = "failed";
                message.Add("物资名称不能为空");
            }
            else if (db.Goods.Count(w => w.GoodsNo == good.GoodsNo) > 0)
            {
                result["code"] = "failed";
                message.Add("物资编号已存在");
            }
            else
            {
                db.Goods.Add(good);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpPut]
        [Author("goods.manage")]
        public Dictionary<string, object> Modify([FromBody]Good good)
        {
            if (string.IsNullOrEmpty(good.GoodsNo))
            {
                result["code"] = "failed";
                message.Add("物资编号不能为空");
            }
            else if (string.IsNullOrEmpty(good.GoodsName))
            {
                result["code"] = "failed";
                message.Add("物资名称不能为空");
            }
            else
            {
                Good newGood = db.Goods.FirstOrDefault(w => w.GoodsId == good.GoodsId);
                if (newGood.GoodsNo != good.GoodsNo && db.Goods.Count(w => w.GoodsNo == good.GoodsNo) > 0)
                {
                    result["code"] = "failed";
                    message.Add("物资编号已存在");
                }
                else
                {
                    Ricky.ObjectCopy.Copy<Good>(good, newGood);
                    db.SaveChanges();
                }
            }
            result["message"] = message;
            return result;
        }
        [HttpDelete]
        [Author("goods.manage")]
        public Dictionary<string, object> Delete(int id)
        {
            Good good = db.Goods.FirstOrDefault(w => w.GoodsId == id);
            db.Goods.Remove(good);
            db.SaveChanges();
            result["message"] = message;
            return result;
        }
    }
}
