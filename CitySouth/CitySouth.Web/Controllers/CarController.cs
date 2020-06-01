using CitySouth.Data;
using Ricky;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace CitySouth.Web.Controllers
{
    public class CarController : BaseController
    {
        [HttpPost]
        [Author("car.list")]
        public Dictionary<string, object> Index([FromBody]SearchModel model)
        {
            var a = from car in db.OwnerCars
                     join b in db.Owners on car.OwnerId equals b.OwnerId
                    join c in db.Houses on b.HouseId equals c.HouseId
                    join d in db.Estates on c.EstateId equals d.EstateId
                    select new
                    {
                        d.EstateName,
                        c.EstateId,
                        c.Building,
                        c.Unit,
                        c.Floor,
                        c.No,
                        c.HouseNo,
                        c.Model,
                        c.Floorage,
                        b.OwnerName,
                        b.CheckInName,
                        b.Phone,
                        car = car
                    };
            if (model.FkId != null && model.FkId > 0)
                a = a.Where(w => w.EstateId == model.FkId);
            else if (!user.IsSuper)
                a = a.Where(w => user.EstateIds.Contains(w.EstateId));
            if (!string.IsNullOrEmpty(model.KeyWord))
                a = a.Where(w => w.HouseNo.Contains(model.KeyWord) || w.Model.Contains(model.KeyWord) || w.OwnerName.Contains(model.KeyWord) || w.Phone.Contains(model.KeyWord) || w.car.Phone.Contains(model.KeyWord) || w.car.UserName.Contains(model.KeyWord) || w.car.CarNumber.Contains(model.KeyWord));
            if (model.SearchDate != null)
            {
                model.SearchDate = model.SearchDate.Value.Date.AddDays(1);
                a = a.Where(w => w.car.ParkingExpireDate < model.SearchDate.Value);
            }
            result["count"] = a.Count();
            var list = a.OrderBy(w => w.EstateId).ThenBy(w => w.Building).ThenBy(w => w.Unit).ThenBy(w => w.Floor).ThenBy(w => w.No).Skip((model.PageIndex - 1) * model.PageSize).Take(model.PageSize).ToList();
            result["datalist"] = list;
            return result;
        }
        [HttpPost]
        [Author("car.export")]
        public HttpResponseMessage Export([FromBody]SearchModel model)
        {
            var a = from car in db.OwnerCars
                    join b in db.Owners on car.OwnerId equals b.OwnerId
                    join c in db.Houses on b.HouseId equals c.HouseId
                    join d in db.Estates on c.EstateId equals d.EstateId
                    select new
                    {
                        d.EstateName,
                        c.EstateId,
                        c.Building,
                        c.Unit,
                        c.Floor,
                        c.No,
                        c.HouseNo,
                        c.Model,
                        c.Floorage,
                        b.OwnerName,
                        b.CheckInName,
                        b.Phone,
                        car = car
                    };
            if (model.FkId != null && model.FkId > 0)
                a = a.Where(w => w.EstateId == model.FkId);
            else if (!user.IsSuper)
                a = a.Where(w => user.EstateIds.Contains(w.EstateId));
            if (!string.IsNullOrEmpty(model.KeyWord))
                a = a.Where(w => w.HouseNo.Contains(model.KeyWord) || w.Model.Contains(model.KeyWord) || w.OwnerName.Contains(model.KeyWord) || w.Phone.Contains(model.KeyWord) || w.car.Phone.Contains(model.KeyWord) || w.car.UserName.Contains(model.KeyWord) || w.car.CarNumber.Contains(model.KeyWord));
            if (model.SearchDate != null)
            {
                model.SearchDate = model.SearchDate.Value.Date.AddDays(1);
                a = a.Where(w => w.car.ParkingExpireDate < model.SearchDate.Value);
            }
            var list = from b in a
                       select new
                       {
                           项目名称 = b.EstateName,
                           房号 = b.HouseNo,
                           户型 = b.Model,
                           建筑面积 = b.Floorage,
                           业主姓名 = b.OwnerName,
                           常住人姓名 = b.CheckInName,
                           联系电话 = b.Phone,
                           车辆使用人 = b.car.UserName,
                           使用人电话 = b.car.Phone,
                           品牌 = b.car.Brand,
                           型号 = b.car.Model,
                           车牌 = b.car.CarNumber,
                           停车费到期日 = b.car.ParkingExpireDate,
                           备注 = b.car.Remark
                       };
            DataTable dt = Common.ToDataTable(list.ToList());
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            Excel excel = new Excel(dt);
            byte[] steam = excel.ToStream();
            MemoryStream ms = new MemoryStream(steam);
            response.Content = new StreamContent(ms);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = HttpUtility.UrlEncode("车主.xlsx")
            };
            return response;
        }
    }
}
