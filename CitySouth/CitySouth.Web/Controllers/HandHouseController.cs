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
    public class HandHouseController : BaseController
    {
        [HttpPost]
        [Author("hand-house.list")]
        public Dictionary<string, object> Index([FromBody]SearchModel model)
        {
            var a = from hand in db.HandHouses
                    join b in db.Owners on hand.OwnerId equals b.OwnerId
                    join c in db.Houses on b.HouseId equals c.HouseId
                    join d in db.Estates on c.EstateId equals d.EstateId
                    select new
                    {
                        d.EstateName,
                        c.EstateId,
                        c.HouseNo,
                        c.Model,
                        c.HouseType,
                        c.Floorage,
                        b.OwnerName,
                        b.Phone,
                        b.PropertyExpireDate,
                        hand
                    };
            if (model.FkId != null && model.FkId > 0)
                a = a.Where(w => w.EstateId == model.FkId);
            else if (!user.IsSuper)
                a = a.Where(w => user.EstateIds.Contains(w.EstateId));
            if (!string.IsNullOrEmpty(model.KeyWord))
                a = a.Where(w => w.HouseNo.Contains(model.KeyWord) || w.OwnerName.Contains(model.KeyWord) || w.Phone.Contains(model.KeyWord) || w.hand.Remark.Contains(model.KeyWord));
            if (model.StartDate != null)
                a = a.Where(w => w.hand.CreateTime >= model.StartDate.Value);
            if (model.EndDate != null)
            {
                model.EndDate = model.EndDate.Value.AddDays(1);
                a = a.Where(w => w.hand.CreateTime < model.EndDate.Value);
            }
            result["count"] = a.Count();
            var list = a.OrderByDescending(w => w.hand.HandId).Skip((model.PageIndex - 1) * model.PageSize).Take(model.PageSize).ToList();
            result["datalist"] = list;
            return result;
        }
        [HttpPost]
        [Author("hand-house.manage")]
        public Dictionary<string, object> Add([FromBody]HandHouse handHouse)
        {
            if (handHouse.OwnerId == 0)
            {
                result["code"] = "failed";
                message.Add("必须选择一个业主");
            }
            else
            {
                handHouse.UserId = user.UserId;
                handHouse.CreateTime = Ricky.Common.NowDate;
                Owner owner = db.Owners.FirstOrDefault(w => w.OwnerId == handHouse.OwnerId);
                owner.HandDate = handHouse.HandDate;
                owner.PropertyStartDate = handHouse.PropertyStartDate;
                owner.PropertyExpireDate = owner.PropertyStartDate;
                House house = db.Houses.FirstOrDefault(w => w.HouseId == owner.HouseId);
                house.HandDate = handHouse.HandDate;
                db.HandHouses.Add(handHouse);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpPut]
        [Author("hand-house.manage")]
        public Dictionary<string, object> Modify([FromBody]HandHouse handHouse)
        {
            HandHouse newHandHouse = db.HandHouses.FirstOrDefault(w => w.HandId == handHouse.HandId);
            if (db.Properties.Count(w => w.OwnerId == newHandHouse.OwnerId) > 0)
            {
                result["code"] = "failed";
                message.Add("已缴纳物业费，无法修改");
            }
            else if (db.WaterAndElectricities.Count(w => w.OwnerId == newHandHouse.OwnerId) > 0)
            {
                result["code"] = "failed";
                message.Add("已缴纳水电费，无法修改");
            }
            else if (db.ElseCosts.Count(w => w.OwnerId == newHandHouse.OwnerId) > 0)
            {
                result["code"] = "failed";
                message.Add("已缴纳费用，无法修改");
            }
            else
            {
                newHandHouse.HandDate = handHouse.HandDate;
                newHandHouse.PropertyStartDate = handHouse.PropertyStartDate;
                newHandHouse.Remark = handHouse.Remark;
                Owner owner = db.Owners.FirstOrDefault(w => w.OwnerId == handHouse.OwnerId);
                owner.HandDate = handHouse.HandDate;
                owner.PropertyStartDate = handHouse.PropertyStartDate;
                owner.PropertyExpireDate = owner.PropertyStartDate;
                House house = db.Houses.FirstOrDefault(w => w.HouseId == owner.HouseId);
                house.HandDate = handHouse.HandDate;
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpDelete]
        [Author("hand-house.delete")]
        public Dictionary<string, object> Delete(int id)
        {
            HandHouse handHouse = db.HandHouses.FirstOrDefault(w => w.HandId == id);
            if (db.Properties.Count(w => w.OwnerId == handHouse.OwnerId) > 0)
            {
                result["code"] = "failed";
                message.Add("已缴纳物业费，无法删除");
            }
            else if (db.WaterAndElectricities.Count(w => w.OwnerId == handHouse.OwnerId) > 0)
            {
                result["code"] = "failed";
                message.Add("已缴纳水电费，无法删除");
            }
            else if (db.ElseCosts.Count(w => w.OwnerId == handHouse.OwnerId) > 0)
            {
                result["code"] = "failed";
                message.Add("已缴纳费用，无法删除");
            }
            else
            {
                Owner owner = db.Owners.FirstOrDefault(w => w.OwnerId == handHouse.OwnerId);
                owner.HandDate = null;
                owner.PropertyStartDate = null;
                House house = db.Houses.FirstOrDefault(w => w.HouseId == owner.HouseId);
                house.HandDate = null;
                db.HandHouses.Remove(handHouse);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
    }
}
