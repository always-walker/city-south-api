using CitySouth.Data;
using CitySouth.Data.Models;
using Newtonsoft.Json;
using Ricky;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CitySouth.Web.Controllers
{
    public class OwnerController : BaseController
    {
        [HttpPost]
        [Author("owner.list")]
        public Dictionary<string, object> Index([FromBody]SearchModel model)
        {
            var a = from b in db.Owners
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
                        owner = b
                    };
            if (model.FkId != null && model.FkId > 0)
                a = a.Where(w => w.EstateId == model.FkId);
            else if (!user.IsSuper)
                a = a.Where(w => user.EstateIds.Contains(w.EstateId));
            if (!string.IsNullOrEmpty(model.KeyWord))
                a = a.Where(w => w.HouseNo.Contains(model.KeyWord) || w.Model.Contains(model.KeyWord) || w.owner.OwnerName.Contains(model.KeyWord) || w.owner.Phone.Contains(model.KeyWord) || w.owner.Remark.Contains(model.KeyWord));
            if (model.SearchDate != null)
            {
                model.SearchDate = model.SearchDate.Value.Date.AddDays(1);
                a = a.Where(w => w.owner.PropertyExpireDate < model.SearchDate.Value);
            }
            if (model.StartDate != null)
                a = a.Where(w => w.owner.HandDate >= model.StartDate.Value);
            if (model.EndDate != null)
            {
                model.EndDate = model.EndDate.Value.AddDays(1);
                a = a.Where(w => w.owner.HandDate < model.EndDate.Value);
            }
            result["count"] = a.Count();
            var list = a.OrderBy(w => w.EstateId).ThenBy(w => w.Building).ThenBy(w => w.Unit).ThenBy(w => w.Floor).ThenBy(w => w.No).Skip((model.PageIndex - 1) * model.PageSize).Take(model.PageSize).ToList();
            foreach (var item in list)
            {
                item.owner.FamilyList = db.OwnerFamilies.Where(w => w.OwnerId == item.owner.OwnerId).ToList();
                item.owner.CarList = db.OwnerCars.Where(w => w.OwnerId == item.owner.OwnerId).ToList();
            }
            result["datalist"] = list;
            return result;
        }
        [HttpPost]
        public Dictionary<string, object> Select([FromBody]SearchModel model)
        {
            var a = from b in db.Owners
                    join c in db.Houses on b.HouseId equals c.HouseId
                    select new
                    {
                        c.HouseNo,
                        c.EstateId,
                        c.HouseType,
                        c.Model,
                        c.Floorage,
                        c.HouseId,
                        b.OwnerId,
                        b.OwnerName,
                        b.CheckInName,
                        b.Phone,
                        b.PropertyStartDate,
                        b.PropertyExpireDate
                    };
            if (model.FkId != null && model.FkId > 0)
                a = a.Where(w => w.EstateId == model.FkId);
            else if (!user.IsSuper)
                a = a.Where(w => user.EstateIds.Contains(w.EstateId));
            if (!string.IsNullOrEmpty(model.KeyWord))
                a = a.Where(w => w.HouseNo.StartsWith(model.KeyWord) || w.CheckInName.Contains(model.KeyWord) || w.OwnerName.Contains(model.KeyWord));
            var list = a.OrderBy(w => w.HouseNo).Take(10).ToList();
            result["datalist"] = list;
            return result;
        }
        [HttpPost]
        [AllowAnonymous]
        public Dictionary<string, object> andCarSelect([FromBody]SearchModel model)
        {
            var a = from b in db.Owners
                    join c in db.Houses on b.HouseId equals c.HouseId
                    select new
                    {
                        c.HouseNo,
                        c.EstateId,
                        c.HouseId,
                        b.OwnerId,
                        b.OwnerName,
                        b.CheckInName,
                        b.Phone,
                        carList = db.OwnerCars.Where(x => x.OwnerId == b.OwnerId).ToList()
                    };
            if (model.FkId != null && model.FkId > 0)
                a = a.Where(w => w.EstateId == model.FkId);
            else if (!user.IsSuper)
                a = a.Where(w => user.EstateIds.Contains(w.EstateId));
            if (!string.IsNullOrEmpty(model.KeyWord))
                a = a.Where(w => w.HouseNo.StartsWith(model.KeyWord) || w.CheckInName.Contains(model.KeyWord) || w.OwnerName.Contains(model.KeyWord));
            var list = a.OrderBy(w => w.HouseNo).Take(10).ToList();
            result["datalist"] = list;
            return result;
        }
        [HttpGet]
        [Author("owner.list")]
        public Dictionary<string, object> expireLog(int id)
        {
            result["datalist"] = db.OwnerPropertyExpireLogs.Where(w => w.OwnerId == id).OrderByDescending(w => w.CreateTime).ToList();
            return result;
        }
        [HttpPost]
        [Author("owner.add")]
        public Dictionary<string, object> Add([FromBody]Owner owner)
        {
            if (owner.HouseId == 0)
            {
                result["code"] = "failed";
                message.Add("必须选择一个房产添加业主");
            }
            else if (db.Houses.Count(w => w.HouseId == owner.HouseId && w.IsHasOwner == false) == 0)
            {
                result["code"] = "failed";
                message.Add("此房产不存在或者已存在业主");
            }
            else if (string.IsNullOrEmpty(owner.OwnerName))
            {
                result["code"] = "failed";
                message.Add("请输入业主姓名");
            }
            else
            {
                owner.PropertyExpireDate = owner.PropertyStartDate;
                db.Owners.Add(owner);
                House house = db.Houses.FirstOrDefault(w => w.HouseId == owner.HouseId);
                house.IsHasOwner = true;
                db.SaveChanges();
                if (owner.FamilyList != null && owner.FamilyList.Count > 0)
                {
                    foreach (OwnerFamily familyItem in owner.FamilyList)
                    {
                        familyItem.OwnerId = owner.OwnerId;
                        db.OwnerFamilies.Add(familyItem);
                    }
                    db.SaveChanges();
                }
                if (owner.CarList != null && owner.CarList.Count > 0)
                {
                    foreach (OwnerCar carItem in owner.CarList)
                    {
                        carItem.OwnerId = owner.OwnerId;
                        db.OwnerCars.Add(carItem);
                    }
                    db.SaveChanges();
                }
            }
            result["message"] = message;
            return result;
        }
        [HttpPut]
        [Author("owner.modify")]
        public Dictionary<string, object> Modify([FromBody]Owner owner)
        {
            if (string.IsNullOrEmpty(owner.OwnerName))
            {
                result["code"] = "failed";
                message.Add("请输入业主姓名");
            }
            else
            {
                Owner newOwner = db.Owners.FirstOrDefault(w => w.OwnerId == owner.OwnerId);
                if ((newOwner.PropertyExpireDate == null && owner.PropertyExpireDate != null) || (newOwner.PropertyExpireDate != null && owner.PropertyExpireDate == null)
            || !(newOwner.PropertyExpireDate.Value.ToString("yyyyMMdd").Equals(owner.PropertyExpireDate.Value.ToString("yyyyMMdd"))))
                {
                    OwnerPropertyExpireLog log = new OwnerPropertyExpireLog();
                    log.UserId = user.UserId;
                    log.UserName = user.UserName;
                    log.OwnerId = newOwner.OwnerId;
                    log.ExpireDate = newOwner.PropertyExpireDate;
                    log.ModifyDate = owner.PropertyExpireDate;
                    log.Remark = owner.ExpireModifyRemark;
                    log.CreateTime = Common.NowDate;
                    db.OwnerPropertyExpireLogs.Add(log);
                }
                Ricky.ObjectCopy.Copy<Owner>(owner, newOwner, new string[] { "HouseId" });
                if (newOwner.PropertyExpireDate == null)
                    newOwner.PropertyExpireDate = newOwner.PropertyStartDate;
                #region 更新家庭信息和房产信息
                if (owner.FamilyList != null && owner.FamilyList.Count > 0)
                {
                    foreach (OwnerFamily familyItem in owner.FamilyList)
                    {
                        if (familyItem.PeopleId == 0)
                        {
                            familyItem.OwnerId = owner.OwnerId;
                            db.OwnerFamilies.Add(familyItem);
                        }
                        else
                        {
                            OwnerFamily newfamilyItem = db.OwnerFamilies.FirstOrDefault(w => w.PeopleId == familyItem.PeopleId);
                            Ricky.ObjectCopy.Copy<OwnerFamily>(familyItem, newfamilyItem, new string[] { "OwnerId" });
                        }
                    }
                    db.SaveChanges();
                }
                if (owner.CarList != null && owner.CarList.Count > 0)
                {
                    foreach (OwnerCar carItem in owner.CarList)
                    {
                        if (carItem.CarId == 0)
                        {
                            carItem.OwnerId = owner.OwnerId;
                            db.OwnerCars.Add(carItem);
                        }
                        else
                        {
                            OwnerCar newcarItem = db.OwnerCars.FirstOrDefault(w => w.CarId == carItem.CarId);
                            Ricky.ObjectCopy.Copy<OwnerCar>(carItem, newcarItem, new string[] { "OwnerId", "ParkingExpireDate" });
                        }
                    }
                    db.SaveChanges();
                }
                #endregion
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpDelete]
        [Author("owner.delete")]
        public Dictionary<string, object> Delete(int id)
        {
            Owner owner = db.Owners.FirstOrDefault(w => w.OwnerId == id);
            if (db.HandHouses.Count(w => w.OwnerId == owner.OwnerId) > 0)
            {
                result["code"] = "failed";
                message.Add("此房产已交房，无法删除。");
            }
            else if (db.Properties.Count(w => w.OwnerId == owner.OwnerId) > 0)
            {
                result["code"] = "failed";
                message.Add("此业主已缴纳物业费，无法删除。");
            }
            else if (db.WaterAndElectricities.Count(w => w.OwnerId == owner.OwnerId) > 0)
            {
                result["code"] = "failed";
                message.Add("此业主已缴纳水电费，无法删除。");
            }
            else if (db.Parkings.Count(w => (from b in db.OwnerCars where b.OwnerId == owner.OwnerId select b.CarId).Contains(w.CarId)) > 0)
            {
                result["code"] = "failed";
                message.Add("此业主已缴纳停车费，无法删除。");
            }
            else
            {
                House house = db.Houses.FirstOrDefault(w => w.HouseId == owner.HouseId);
                var ownerCar = from b in db.OwnerCars where b.OwnerId == owner.OwnerId select b;
                db.OwnerCars.RemoveRange(ownerCar);
                var ownerFamily = from b in db.OwnerFamilies where b.OwnerId == owner.OwnerId select b;
                db.OwnerFamilies.RemoveRange(ownerFamily);
                db.Owners.Remove(owner);
                house.IsHasOwner = false;
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpDelete]
        [Author("owner.modify")]
        public Dictionary<string, object> DeletePeople(int id)
        {
            OwnerFamily family = db.OwnerFamilies.FirstOrDefault(w => w.PeopleId == id);
            db.OwnerFamilies.Remove(family);
            db.SaveChanges();
            return result;
        }
        [HttpDelete]
        [Author("owner.modify")]
        public Dictionary<string, object> DeleteCar(int id)
        {
            if (db.Parkings.Count(w => w.CarId == id) > 0)
            {
                result["code"] = "failed";
                message.Add("此车辆已缴纳停车费，无法删除。");
            }
            else
            {
                OwnerCar car = db.OwnerCars.FirstOrDefault(w => w.CarId == id);
                db.OwnerCars.Remove(car);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpPost]
        [Author("owner.import")]
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
                            try
                            {
                                Owner owner = new Owner();
                                owner.HouseId = house.HouseId;
                                owner.OwnerName = dr[2].ToString();
                                owner.CheckInName = dr[3].ToString();
                                owner.CardId = dr[4].ToString();
                                owner.Phone = dr[5].ToString();
                                owner.Occupation = dr[6].ToString();
                                DateTime PropertyStartDate = DateTime.MinValue;
                                if (DateTime.TryParse(dr[7].ToString(), out PropertyStartDate))
                                    owner.PropertyStartDate = PropertyStartDate;
                                DateTime PropertyExpireDate = DateTime.MinValue;
                                if (DateTime.TryParse(dr[8].ToString(), out PropertyExpireDate))
                                    owner.PropertyExpireDate = PropertyExpireDate;
                                DateTime HandDate = DateTime.MinValue;
                                if (DateTime.TryParse(dr[9].ToString(), out HandDate))
                                    owner.HandDate = HandDate;
                                DateTime RenovationDate = DateTime.MinValue;
                                if (DateTime.TryParse(dr[10].ToString(), out RenovationDate))
                                    owner.RenovationDate = RenovationDate;
                                DateTime CheckInDate = DateTime.MinValue;
                                if (DateTime.TryParse(dr[11].ToString(), out CheckInDate))
                                    owner.CheckInDate = CheckInDate;
                                owner.UseInfo = dr[12].ToString();
                                owner.Remark = dr[13].ToString();
                                if (db.Owners.Count(w => w.HouseId == owner.HouseId) == 0)
                                {
                                    db.Owners.Add(owner);
                                    db.SaveChanges();
                                    dr[14] = "成功";
                                    dr[15] = "新增";
                                }
                                else
                                {
                                    Owner newOwner = db.Owners.First(w => w.HouseId == owner.HouseId);
                                    Ricky.ObjectCopy.Copy<Owner>(owner, newOwner, new string[] { "OwnerId", "CheckInType" });
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
            db.Database.ExecuteSqlCommand("update House set IsHasOwner=1 where IsHasOwner=0 and HouseId in(select HouseId from [Owner])");
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/upload/owner");
            if (!Directory.Exists(fileSaveLocation))
                Directory.CreateDirectory(fileSaveLocation);
            string Name = fileName.Substring(0, fileName.LastIndexOf('.'));
            Excel excel = new Excel(dt);
            string saveFileName = string.Format("{1}{2}.xlsx", fileSaveLocation, Name, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            excel.Save(fileSaveLocation + "\\" + saveFileName);
            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, filename = saveFileName });
        }
        [HttpGet]
        [Author("owner.import")]
        public HttpResponseMessage ImportFile([FromUri]string filename)
        {
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/upload/owner");
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
        [Author("owner.export")]
        public HttpResponseMessage Export([FromBody]SearchModel model)
        {
            var a = from b in db.Owners
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
                        owner = b
                    };
            if (model.FkId != null && model.FkId > 0)
                a = a.Where(w => w.EstateId == model.FkId);
            else if (!user.IsSuper)
                a = a.Where(w => user.EstateIds.Contains(w.EstateId));
            if (!string.IsNullOrEmpty(model.KeyWord))
                a = a.Where(w => w.HouseNo.Contains(model.KeyWord) || w.Model.Contains(model.KeyWord) || w.owner.OwnerName.Contains(model.KeyWord) || w.owner.Phone.Contains(model.KeyWord) || w.owner.Remark.Contains(model.KeyWord));
            if (model.SearchDate != null)
            {
                model.SearchDate = model.SearchDate.Value.Date.AddDays(1);
                a = a.Where(w => w.owner.PropertyExpireDate < model.SearchDate.Value);
            }
            if (model.StartDate != null)
                a = a.Where(w => w.owner.HandDate >= model.StartDate.Value);
            if (model.EndDate != null)
            {
                model.EndDate = model.EndDate.Value.AddDays(1);
                a = a.Where(w => w.owner.HandDate < model.EndDate.Value);
            }
            var list = from b in a
                       select new
                       {
                           项目名称 = b.EstateName,
                           房号 = b.HouseNo,
                           户型 = b.Model,
                           建筑面积 = b.Floorage,
                           业主姓名 = b.owner.OwnerName,
                           常住人姓名 = b.owner.CheckInName,
                           联系电话 = b.owner.Phone,
                           身份证号 = b.owner.CardId,
                           单位职业 = b.owner.Occupation,
                           交房日期 = b.owner.HandDate,
                           装修日期 = b.owner.RenovationDate,
                           入住日期 =b.owner.CheckInDate,
                           物业起始日 = b.owner.PropertyStartDate,
                           物业到期日 = b.owner.PropertyExpireDate,
                           使用信息 = b.owner.UseInfo,
                           备注 = b.owner.Remark
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
                FileName = HttpUtility.UrlEncode("物业费.xlsx")
            };
            return response;
        }
    }
}
