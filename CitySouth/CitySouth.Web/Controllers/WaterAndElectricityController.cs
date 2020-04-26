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
    public class WaterAndElectricityController : BaseController
    {
        [HttpPost]
        [Author("water-electricity.list")]
        public Dictionary<string, object> Index([FromBody]SearchModel model)
        {
            var a = from waterAndE in db.WaterAndElectricities
                    join b in db.Owners on waterAndE.OwnerId equals b.OwnerId
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
                        waterAndE
                    };
            if (model.FkId != null && model.FkId > 0)
                a = a.Where(w => w.EstateId == model.FkId);
            else if (!user.IsSuper)
                a = a.Where(w => user.EstateIds.Contains(w.EstateId));
            if (!string.IsNullOrEmpty(model.KeyWord))
                a = a.Where(w => w.HouseNo.Contains(model.KeyWord) || w.OwnerName.Contains(model.KeyWord) || w.CheckInName.Contains(model.KeyWord) || w.Phone.Contains(model.KeyWord) || w.waterAndE.VoucherNo.Contains(model.KeyWord) || w.waterAndE.ReceiptNo.Contains(model.KeyWord) || w.waterAndE.Remark.Contains(model.KeyWord));
            if (!string.IsNullOrEmpty(model.type))
                a = a.Where(w => w.waterAndE.FeeType == model.type);
            if (model.StartDate != null)
                a = a.Where(w => w.waterAndE.FeeDate >= model.StartDate.Value);
            if (model.EndDate != null)
            {
                model.EndDate = model.EndDate.Value.AddDays(1);
                a = a.Where(w => w.waterAndE.FeeDate < model.EndDate.Value);
            }
            result["count"] = a.Count();
            var list = a.OrderByDescending(w => w.waterAndE.FeeId).Skip((model.PageIndex - 1) * model.PageSize).Take(model.PageSize).ToList();
            result["datalist"] = list;
            return result;
        }
        [HttpPost]
        [Author("water-electricity.manage")]
        public Dictionary<string, object> Add([FromBody]WaterAndElectricity waterAndE)
        {
            if (waterAndE.OwnerId == 0)
            {
                result["code"] = "failed";
                message.Add("必须选择一个业主");
            }
            else if (waterAndE.Quantity < waterAndE.LastQuantity)
            {
                result["code"] = "failed";
                message.Add("本月表量不能小于上月表量");
            }
            else
            {
                waterAndE.UserId = user.UserId;
                waterAndE.OprationName = user.UserName;
                waterAndE.CreateTime = Ricky.Common.NowDate;
                if (waterAndE.Status == 1)
                {
                    waterAndE.PayTime = waterAndE.CreateTime;
                    Owner owner = db.Owners.FirstOrDefault(w => w.OwnerId == waterAndE.OwnerId);
                    waterAndE.PayerName = string.IsNullOrEmpty(owner.CheckInName) ? owner.OwnerName : owner.CheckInName;
                }
                db.WaterAndElectricities.Add(waterAndE);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpPut]
        [Author("water-electricity.manage")]
        public Dictionary<string, object> Modify([FromBody]WaterAndElectricity waterAndE)
        {
            if (waterAndE.Quantity < waterAndE.LastQuantity)
            {
                result["code"] = "failed";
                message.Add("本月表量不能小于上月表量");
            }
            else
            {
                WaterAndElectricity newWaterAndE = db.WaterAndElectricities.FirstOrDefault(w => w.FeeId == waterAndE.FeeId);
                if (newWaterAndE.Status > -1)
                {
                    newWaterAndE.ConfigId = waterAndE.ConfigId;
                    newWaterAndE.ReceiptNo = waterAndE.ReceiptNo;
                    newWaterAndE.VoucherNo = waterAndE.VoucherNo;
                    newWaterAndE.PayWay = waterAndE.PayWay;
                    newWaterAndE.Remark = waterAndE.Remark;
                    if (newWaterAndE.Status == 0)
                    {
                        newWaterAndE.FeeDate = waterAndE.FeeDate;
                        newWaterAndE.Quantity = waterAndE.Quantity;
                        newWaterAndE.LastQuantity = waterAndE.LastQuantity;
                        newWaterAndE.Amount = waterAndE.Amount;
                    }
                    if (newWaterAndE.Status == 0 && waterAndE.Status == 1)
                    {
                        newWaterAndE.OprationName = user.UserName;
                        newWaterAndE.PayTime = Ricky.Common.NowDate;
                        newWaterAndE.Status = waterAndE.Status;
                        Owner owner = db.Owners.FirstOrDefault(w => w.OwnerId == newWaterAndE.OwnerId);
                        newWaterAndE.PayerName = string.IsNullOrEmpty(owner.CheckInName) ? owner.OwnerName : owner.CheckInName;
                    }
                    db.SaveChanges();
                }
                else
                {
                    result["code"] = "failed";
                    message.Add("此单已作废");
                }
            }
            result["message"] = message;
            return result;
        }
        [HttpDelete]
        [Author("water-electricity.delete")]
        public Dictionary<string, object> Delete(int id)
        {
            WaterAndElectricity waterAndE = db.WaterAndElectricities.FirstOrDefault(w => w.FeeId == id);
            if (waterAndE.Status > -1)
            {
                if (waterAndE.Status == 1)
                {
                    waterAndE.PayTime = null;
                }
                waterAndE.Status = -1;
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
        [HttpPost]
        public Dictionary<string, object> PreQuantity([FromBody]SearchModel model)
        {
            DateTime startDate = DateTime.Parse(model.SearchDate.Value.AddMonths(-1).ToString("yyyy-MM") + "-01");
            DateTime endDate = startDate.AddMonths(1);
            WaterAndElectricity waterAndE = db.WaterAndElectricities.FirstOrDefault(w => w.OwnerId == model.FkId && w.FeeType == model.type && w.FeeDate >= startDate && w.FeeDate < endDate && w.Status > -1);
            if (waterAndE != null)
            {
                result["LastQuantity"] = waterAndE.Quantity;
            }
            else
            {
                result["LastQuantity"] = null;
            }
            return result;
        }
    }
}
