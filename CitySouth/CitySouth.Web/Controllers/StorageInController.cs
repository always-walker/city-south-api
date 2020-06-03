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
    public class StorageInController : BaseController
    {
        [HttpPost]
        [Author("storage-in.list")]
        public Dictionary<string, object> Index([FromBody]SearchModel model)
        {
            var a = from b in db.GoodsReceipts select b;
            if (!string.IsNullOrEmpty(model.KeyWord))
                a = a.Where(w => w.ReceiptNo.Contains(model.KeyWord) || w.Purchaser.Contains(model.KeyWord));
            result["count"] = a.Count();
            var list = a.ToList();
            result["datalist"] = list;
            return result;
        }
        [HttpPost]
        [Author("storage-in.manage")]
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
        [Author("storage-in.manage")]
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
        [Author("storage-in.manage")]
        public Dictionary<string, object> Delete(int id)
        {
            if (db.GoodsStorages.Count(w => w.GoodsId == id) > 0
                || db.GoodsReceiptDetails.Count(w => w.GoodsId == id) > 0
                || db.GoodsOutDetails.Count(w => w.GoodsId == id) > 0)
            {
                result["code"] = "failed";
                message.Add("物资已使用不能删除");
            }
            else
            {
                Good good = db.Goods.FirstOrDefault(w => w.GoodsId == id);
                db.Goods.Remove(good);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
    }
}
