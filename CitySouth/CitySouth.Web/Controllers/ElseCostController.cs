using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CitySouth.Data;
using CitySouth.Data.Models;

namespace CitySouth.Web.Controllers
{
    public class ElseCostController : BaseController
    {
        [HttpGet]
        public Dictionary<string, object> Group()
        {
            var a = from b in db.ElseCosts group b by b.CostName into g select g.Key;
            result["groups"] = a.ToList();
            return result;
        }
        [HttpPost]
        [Author("else-cost.list")]
        public Dictionary<string, object> Index([FromBody]SearchModel model)
        {
            var a = from elseCost in db.ElseCosts
                    join b in db.Owners on elseCost.OwnerId equals b.OwnerId
                    join c in db.Houses on b.HouseId equals c.HouseId
                    join d in db.Estates on c.EstateId equals d.EstateId
                    select new
                    {
                        d.EstateName,
                        c.EstateId,
                        c.HouseNo,
                        b.OwnerName,
                        b.CheckInName,
                        b.Phone,
                        elseCost
                    };
            if (model.FkId != null && model.FkId > 0)
                a = a.Where(w => w.EstateId == model.FkId);
            else if (!user.IsSuper)
                a = a.Where(w => user.EstateIds.Contains(w.EstateId));
            if (!string.IsNullOrEmpty(model.KeyWord))
                a = a.Where(w => w.HouseNo.Contains(model.KeyWord) || w.OwnerName.Contains(model.KeyWord) || w.CheckInName.Contains(model.KeyWord) || w.Phone.Contains(model.KeyWord) || w.elseCost.VoucherNo.Contains(model.KeyWord) || w.elseCost.ReceiptNo.Contains(model.KeyWord) || w.elseCost.Remark.Contains(model.KeyWord));
            if (!string.IsNullOrEmpty(model.type))
                a = a.Where(w => w.elseCost.CostName == model.type);
            if (model.StartDate != null)
                a = a.Where(w => w.elseCost.CreateTime >= model.StartDate.Value);
            if (model.EndDate != null)
            {
                model.EndDate = model.EndDate.Value.AddDays(1);
                a = a.Where(w => w.elseCost.CreateTime < model.EndDate.Value);
            }
            result["count"] = a.Count();
            var list = a.OrderByDescending(w => w.elseCost.ElseCostId).Skip((model.PageIndex - 1) * model.PageSize).Take(model.PageSize).ToList();
            result["datalist"] = list;
            return result;
        }
        [HttpPost]
        [Author("else-cost.manage")]
        public Dictionary<string, object> Add([FromBody]ElseCost elseCost)
        {
            if (elseCost.OwnerId == 0)
            {
                result["code"] = "failed";
                message.Add("必须选择一个业主");
            }
            else
            {
                elseCost.UserId = user.UserId;
                elseCost.OprationName = user.UserName;
                elseCost.CreateTime = Ricky.Common.NowDate;
                if (elseCost.Status == 1)
                {
                    elseCost.PayTime = elseCost.CreateTime;
                    Owner owner = db.Owners.FirstOrDefault(w => w.OwnerId == elseCost.OwnerId);
                    elseCost.PayerName = string.IsNullOrEmpty(owner.CheckInName) ? owner.OwnerName : owner.CheckInName;
                }
                db.ElseCosts.Add(elseCost);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpPut]
        [Author("else-cost.manage")]
        public Dictionary<string, object> Modify([FromBody]ElseCost elseCost)
        {
            ElseCost newElseCost = db.ElseCosts.FirstOrDefault(w => w.ElseCostId == elseCost.ElseCostId);
            if (elseCost.Status > -1)
            {
                newElseCost.ConfigId = elseCost.ConfigId;
                newElseCost.ReceiptNo = elseCost.ReceiptNo;
                newElseCost.VoucherNo = elseCost.VoucherNo;
                newElseCost.PayWay = elseCost.PayWay;
                newElseCost.Remark = elseCost.Remark;
                if (newElseCost.Status == 0)
                {
                    newElseCost.CostName = elseCost.CostName;
                    newElseCost.StartDate = elseCost.StartDate;
                    newElseCost.EndDate = elseCost.EndDate;
                    newElseCost.UnitPrice = elseCost.UnitPrice;
                    newElseCost.Amount = elseCost.Amount;
                }
                if (newElseCost.Status == 0 && elseCost.Status == 1)
                {
                    newElseCost.PayTime = Ricky.Common.NowDate;
                    newElseCost.Status = elseCost.Status;
                    newElseCost.OprationName = user.UserName;
                    Owner owner = db.Owners.FirstOrDefault(w => w.OwnerId == newElseCost.OwnerId);
                    newElseCost.PayerName = string.IsNullOrEmpty(owner.CheckInName) ? owner.OwnerName : owner.CheckInName;
                }
                db.SaveChanges();
            }
            else
            {
                result["code"] = "failed";
                message.Add("此单已作废");
            }
            result["message"] = message;
            return result;
        }
        [HttpDelete]
        [Author("else-cost.delete")]
        public Dictionary<string, object> Delete(int id)
        {
            ElseCost elseCost = db.ElseCosts.FirstOrDefault(w => w.ElseCostId == id);
            if (elseCost.Status > -1)
            {
                if (elseCost.Status == 1)
                {
                    elseCost.PayTime = null;
                }
                elseCost.Status = -1;
                db.SaveChanges();
            }
            else
            {
                result["code"] = "failed";
                message.Add("此单已作废");
            }
            result["message"] = message;
            return result;
        }
    }
}
