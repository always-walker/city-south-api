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
    public class ParkingController : BaseController
    {
        [HttpPost]
        [Author("parking.list")]
        public Dictionary<string, object> Index([FromBody]SearchModel model)
        {
            var a = from parking in db.Parkings join car in db.OwnerCars on parking.CarId equals car.CarId
                    join b in db.Owners on car.OwnerId equals b.OwnerId
                    join c in db.Houses on b.HouseId equals c.HouseId
                    join d in db.Estates on c.EstateId equals d.EstateId
                    select new
                    {
                        d.EstateName,
                        c.EstateId,
                        c.HouseId,
                        c.HouseNo,
                        b.OwnerId,
                        b.OwnerName,
                        b.CheckInName,
                        b.Phone,
                        car.ParkingExpireDate,
                        car.CarNumber,
                        car.Brand,
                        car.Model,
                        parking
                    };
            if (model.FkId != null && model.FkId > 0)
                a = a.Where(w => w.EstateId == model.FkId);
            else if (!user.IsSuper)
                a = a.Where(w => user.EstateIds.Contains(w.EstateId));
            if (!string.IsNullOrEmpty(model.KeyWord))
                a = a.Where(w => w.HouseNo.Contains(model.KeyWord) || w.OwnerName.Contains(model.KeyWord) || w.CheckInName.Contains(model.KeyWord) || w.Phone.Contains(model.KeyWord) || w.parking.VoucherNo.Contains(model.KeyWord) || w.parking.ReceiptNo.Contains(model.KeyWord) || w.parking.Remark.Contains(model.KeyWord));
            if (model.StartDate != null)
                a = a.Where(w => w.parking.CreateTime >= model.StartDate.Value);
            if (model.EndDate != null)
            {
                model.EndDate = model.EndDate.Value.AddDays(1);
                a = a.Where(w => w.parking.CreateTime < model.EndDate.Value);
            }
            result["count"] = a.Count();
            var list = a.OrderByDescending(w => w.parking.ParkingId).Skip((model.PageIndex - 1) * model.PageSize).Take(model.PageSize).ToList();
            result["datalist"] = list;
            return result;
        }
        [HttpPost]
        [Author("parking.manage")]
        public Dictionary<string, object> Add([FromBody]Parking parking)
        {
            if (parking.CarId == 0)
            {
                result["code"] = "failed";
                message.Add("必须选择一个车子");
            }
            else
            {
                parking.UserId = user.UserId;
                parking.OprationName = user.UserName;
                parking.CreateTime = Ricky.Common.NowDate;
                if (parking.Status == 1)
                {
                    parking.PayTime = parking.CreateTime;
                    OwnerCar car = db.OwnerCars.FirstOrDefault(w => w.CarId == parking.CarId);
                    Owner owner = db.Owners.FirstOrDefault(w => w.OwnerId == car.OwnerId);
                    parking.PayerName = string.IsNullOrEmpty(owner.CheckInName) ? owner.OwnerName : owner.CheckInName;
                    car.ParkingExpireDate = parking.EndDate;
                }
                db.Parkings.Add(parking);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpPut]
        [Author("parking.manage")]
        public Dictionary<string, object> Modify([FromBody]Parking parking)
        {
            Parking newParking = db.Parkings.FirstOrDefault(w => w.ParkingId == parking.ParkingId);
            if (parking.Status > -1)
            {
                newParking.ReceiptNo = parking.ReceiptNo;
                newParking.VoucherNo = parking.VoucherNo;
                newParking.PayWay = parking.PayWay;
                newParking.Remark = parking.Remark;
                newParking.StartDate = parking.StartDate;
                newParking.EndDate = parking.EndDate;
                if (newParking.Status == 0 && parking.Status == 1)
                {
                    newParking.OprationName = user.UserName;
                    newParking.PayTime = Ricky.Common.NowDate;
                    newParking.Status = parking.Status;
                    OwnerCar car = db.OwnerCars.FirstOrDefault(w => w.CarId == newParking.CarId);
                    Owner owner = db.Owners.FirstOrDefault(w => w.OwnerId == car.OwnerId);
                    newParking.PayerName = string.IsNullOrEmpty(owner.CheckInName) ? owner.OwnerName : owner.CheckInName;
                    car.ParkingExpireDate = parking.EndDate;
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
        [Author("parking.delete")]
        public Dictionary<string, object> Delete(int id)
        {
            Parking parking = db.Parkings.FirstOrDefault(w => w.ParkingId == id);
            if (parking.Status > -1)
            {
                if (parking.Status == 1)
                {
                    parking.PayTime = null;
                    OwnerCar car = db.OwnerCars.FirstOrDefault(w => w.CarId == parking.CarId);
                    if (db.Parkings.Count(w => w.CarId == parking.CarId && w.Status == 1 && w.ParkingId != parking.ParkingId) > 0)
                        car.ParkingExpireDate = db.Parkings.Where(w => w.CarId == parking.CarId && w.Status == 1 && w.ParkingId != parking.ParkingId).Max(w => w.EndDate);
                    else
                        car.ParkingExpireDate = null;
                }
                parking.Status = -1;
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
