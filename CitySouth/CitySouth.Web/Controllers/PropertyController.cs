using CitySouth.Data;
using CitySouth.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CitySouth.Web.Controllers
{
    public class PropertyController : BaseController
    {
        [HttpPost]
        [Author("property.list")]
        public Dictionary<string, object> Index([FromBody]SearchModel model)
        {
            var a = from property in db.Properties
                    join b in db.Owners on property.OwnerId equals b.OwnerId
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
                        b.CheckInName,
                        b.Phone,
                        b.PropertyExpireDate,
                        property
                    };
            if (model.FkId != null && model.FkId > 0)
                a = a.Where(w => w.EstateId == model.FkId);
            else if (!user.IsSuper)
                a = a.Where(w => user.EstateIds.Contains(w.EstateId));
            if (!string.IsNullOrEmpty(model.KeyWord))
                a = a.Where(w => w.HouseNo.Contains(model.KeyWord) || w.Model.Contains(model.KeyWord) || w.OwnerName.Contains(model.KeyWord) || w.CheckInName.Contains(model.KeyWord) || w.Phone.Contains(model.KeyWord) || w.property.VoucherNo.Contains(model.KeyWord) || w.property.ReceiptNo.Contains(model.KeyWord) || w.property.Remark.Contains(model.KeyWord));
            if (model.StartDate != null)
                a = a.Where(w => w.property.CreateTime >= model.StartDate.Value);
            if (model.EndDate != null)
            {
                model.EndDate = model.EndDate.Value.AddDays(1);
                a = a.Where(w => w.property.CreateTime < model.EndDate.Value);
            }
            result["count"] = a.Count();
            var list = a.OrderByDescending(w => w.property.PropertyId).Skip((model.PageIndex - 1) * model.PageSize).Take(model.PageSize).ToList();
            result["datalist"] = list;
            return result;
        }
        [HttpPost]
        [Author("property.manage")]
        public Dictionary<string, object> Add([FromBody]Property property)
        {
            if (property.OwnerId == 0)
            {
                result["code"] = "failed";
                message.Add("必须选择一个业主");
            }
            else
            {
                property.UserId = user.UserId;
                property.OprationName = user.UserName;
                property.CreateTime = Ricky.Common.NowDate;
                if (property.Status == 1)
                {
                    property.PayTime = property.CreateTime;
                    Owner owner = db.Owners.FirstOrDefault(w => w.OwnerId == property.OwnerId);
                    property.PayerName = string.IsNullOrEmpty(owner.CheckInName) ? owner.OwnerName : owner.CheckInName;
                    owner.PropertyExpireDate = property.EndDate;
                }
                new sysUserLog().add<Property>(db, user, "新增物业费缴费", property);
                db.Properties.Add(property);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpPut]
        [Author("property.manage")]
        public Dictionary<string, object> Modify([FromBody]Property property)
        {
            Property newProperty = db.Properties.FirstOrDefault(w => w.PropertyId == property.PropertyId);
            new sysUserLog().add<Property>(db, user, "修改物业费缴费", newProperty, property);
            if(newProperty.Status > -1)
            {
                newProperty.ReceiptNo = property.ReceiptNo;
                newProperty.VoucherNo = property.VoucherNo;
                newProperty.PayWay = property.PayWay;
                newProperty.Remark = property.Remark;
                newProperty.StartDate = property.StartDate;
                newProperty.EndDate = property.EndDate;
                if (newProperty.Status == 0 && property.Status == 1)
                {
                    newProperty.PayTime = Ricky.Common.NowDate;
                    newProperty.Status = property.Status;
                    newProperty.OprationName = user.UserName;
                    Owner owner = db.Owners.FirstOrDefault(w => w.OwnerId == newProperty.OwnerId);
                    newProperty.PayerName = string.IsNullOrEmpty(owner.CheckInName) ? owner.OwnerName : owner.CheckInName;
                    owner.PropertyExpireDate = property.EndDate;
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
        [Author("property.delete")]
        public Dictionary<string, object> Delete(int id)
        {
            Property property = db.Properties.FirstOrDefault(w => w.PropertyId == id);
            new sysUserLog().add<Property>(db, user, "作废物业费缴费", property);
            if (property.Status > -1)
            {
                if (property.Status == 1)
                {
                    property.PayTime = null;
                    Owner owner = db.Owners.FirstOrDefault(w => w.OwnerId == property.OwnerId);
                    int MonthCount = (property.EndDate.Year - property.StartDate.Year) * 12 + property.EndDate.Month - property.StartDate.Month;
                    owner.PropertyExpireDate = owner.PropertyExpireDate.Value.AddMonths(0 - MonthCount);
                }
                property.Status = -1;
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
