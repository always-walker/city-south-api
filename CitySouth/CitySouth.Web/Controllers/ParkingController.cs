using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CitySouth.Data;
using CitySouth.Data.Models;
using System.Web;
using System.IO;
using System.Net.Http.Headers;
using Ricky;
using System.Data;
using System.Threading.Tasks;

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
        [HttpPost]
        [Author("parking.import")]
        public async Task<HttpResponseMessage> Import()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            MultipartMemoryStreamProvider provider = await Request.Content.ReadAsMultipartAsync();
            HttpContent content = provider.Contents.First();
            Stream stream = await content.ReadAsStreamAsync();
            string fileName = content.Headers.ContentDisposition.FileName.Trim('"');
            DataTable dt = Excel.ExcelToDataTable(fileName, stream);
            dt.Columns.Add("导入状态");
            dt.Columns.Add("导入备注");
            foreach (DataRow dr in dt.Rows)
            {
                string EstateName = dr[0].ToString();
                if (!string.IsNullOrEmpty(EstateName))
                {
                    Estate estate = db.Estates.FirstOrDefault(w => w.EstateName == EstateName);
                    if (estate != null)
                    {
                        string HouseNo = dr[1].ToString();
                        House house = db.Houses.FirstOrDefault(w => w.EstateId == estate.EstateId && w.HouseNo == HouseNo);
                        if (house != null)
                        {
                            Owner owner = db.Owners.FirstOrDefault(w => w.HouseId == house.HouseId);
                            if (owner != null)
                            {
                                string CarNumber = dr[2].ToString();
                                OwnerCar car = db.OwnerCars.FirstOrDefault(w => w.OwnerId == owner.OwnerId && w.CarNumber == CarNumber);
                                if(car == null)
                                {
                                    car = new OwnerCar();
                                    car.OwnerId = owner.OwnerId;
                                    car.CarNumber = CarNumber;
                                    db.OwnerCars.Add(car);
                                    db.SaveChanges();
                                }
                                try
                                {
                                    Parking parking = new Parking();
                                    parking.CarId = car.CarId;
                                    parking.PayerName = owner.CheckInName;
                                    parking.UnitPrice = decimal.Parse(dr[3].ToString());
                                    CostConfig config = db.CostConfigs.FirstOrDefault(w => w.EstateId == estate.EstateId && w.ConfigType == "parking" && w.UnitPrice == parking.UnitPrice);
                                    if (config != null)
                                        parking.ConfigId = config.ConfigId;
                                    parking.MonthCount = double.Parse(dr[4].ToString());
                                    parking.Amount = decimal.Parse(dr[5].ToString());
                                    parking.CreateTime = DateTime.Now;
                                    DateTime PayTime = DateTime.MinValue;
                                    if (DateTime.TryParse(dr[6].ToString(), out PayTime))
                                        parking.PayTime = PayTime;
                                    DateTime StartDate = DateTime.MinValue;
                                    if (DateTime.TryParse(dr[7].ToString(), out StartDate))
                                        parking.StartDate = StartDate;
                                    DateTime EndDate = DateTime.MinValue;
                                    if (DateTime.TryParse(dr[8].ToString(), out EndDate))
                                        parking.EndDate = EndDate;
                                    parking.ReceiptNo = dr[9].ToString();
                                    parking.VoucherNo = dr[10].ToString();
                                    parking.OprationName = dr[11].ToString();
                                    parking.PayWay = dr[12].ToString();
                                    parking.Remark = dr[13].ToString();
                                    if (parking.PayTime != null)
                                        parking.Status = 1;
                                    if (db.Parkings.Count(w => w.CarId == car.CarId && w.Amount == parking.Amount && w.MonthCount == parking.MonthCount && w.StartDate == parking.StartDate && w.EndDate == parking.EndDate) == 0)
                                    {
                                        db.Parkings.Add(parking);
                                        db.SaveChanges();
                                        dr[14] = "成功";
                                        dr[15] = "新增";
                                    }
                                    else
                                    {
                                        Parking newParking = db.Parkings.First(w => w.CarId == car.CarId && w.Amount == parking.Amount && w.MonthCount == parking.MonthCount && w.StartDate == parking.StartDate && w.EndDate == parking.EndDate);
                                        Ricky.ObjectCopy.Copy<Parking>(parking, newParking, new string[] { "ParkingId", "PayerName", "UserId" });
                                        db.SaveChanges();
                                        dr[14] = "成功";
                                        dr[15] = "修改";
                                    }
                                }
                                catch (Exception e)
                                {
                                    dr[14] = "失败";
                                    dr[15] = e.Message;
                                }
                            }
                            else
                            {
                                dr[14] = "失败";
                                dr[15] = "没有找到业主";
                            }
                        }
                        else
                        {
                            dr[14] = "失败";
                            dr[15] = "沒有找到此房产";
                        }
                    }
                    else
                    {
                        dr[14] = "失败";
                        dr[15] = "沒有找到此小区";
                    }
                }
                else
                {
                    dr[14] = "失败";
                    dr[15] = "缺少所属小区名称";
                }
            }
            db.Database.ExecuteSqlCommand("update OwnerCar set ParkingExpireDate=t.EndDate from (select CarId,MAX(EndDate) EndDate from Parking group by CarId)t where OwnerCar.CarId=t.CarId");
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/upload/parking");
            if (!Directory.Exists(fileSaveLocation))
                Directory.CreateDirectory(fileSaveLocation);
            string Name = fileName.Substring(0, fileName.LastIndexOf('.'));
            Excel excel = new Excel(dt);
            string saveFileName = string.Format("{1}{2}.xlsx", fileSaveLocation, Name, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            excel.Save(fileSaveLocation + "\\" + saveFileName);
            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, filename = saveFileName });
        }
        [HttpGet]
        [Author("parking.import")]
        public HttpResponseMessage ImportFile([FromUri]string filename)
        {
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/upload/parking");
            var filePath = fileSaveLocation + "\\" + filename;
            if (File.Exists(filePath))
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(new FileStream(filePath, FileMode.Open, FileAccess.Read));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = HttpUtility.UrlEncode(filename)
                };
                return response;
            }
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "");
        }
        [HttpPost]
        [Author("parking.export")]
        public HttpResponseMessage Export([FromBody]SearchModel model)
        {
            var a = from parking in db.Parkings
                    join car in db.OwnerCars on parking.CarId equals car.CarId
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
            var list = from b in a
                       select new
                       {
                           项目名称 = b.EstateName,
                           房号 = b.HouseNo,
                           业主姓名 = b.OwnerName,
                           缴费人姓名 = b.parking.PayerName,
                           联系电话 = b.Phone,
                           车牌号 = b.CarNumber,
                           品牌 = b.Brand,
                           型号 = b.Model,
                           停车费到期日 = b.ParkingExpireDate,
                           单价 = b.parking.UnitPrice,
                           缴费月数 = b.parking.MonthCount,
                           缴费金额 = b.parking.Amount,
                           缴费时间 = b.parking.CreateTime,
                           支付时间 = b.parking.PayTime,
                           服务开始时间 = b.parking.StartDate,
                           服务结束时间 = b.parking.EndDate,
                           收据号码 = b.parking.ReceiptNo,
                           凭证号码 = b.parking.VoucherNo,
                           操作员 = b.parking.OprationName,
                           缴费方式 = b.parking.PayWay,
                           备注 = b.parking.Remark
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
                FileName = HttpUtility.UrlEncode("停车费.xlsx")
            };
            return response;
        }
    }
}
