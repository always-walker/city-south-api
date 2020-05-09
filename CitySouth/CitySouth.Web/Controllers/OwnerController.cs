using CitySouth.Data;
using CitySouth.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
                Ricky.ObjectCopy.Copy<Owner>(owner, newOwner, new string[] { "HouseId" });
                if (newOwner.PropertyExpireDate == null)
                    newOwner.PropertyExpireDate = newOwner.PropertyStartDate;
                //更新家庭信息和房产信息
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
        [AllowAnonymous]
        public async Task<HttpResponseMessage> import()
        {
            // Check whether the POST operation is MultiPart?
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            // Prepare CustomMultipartFormDataStreamProvider in which our multipart form
            // data will be loaded.
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/upload/owner");
            if (!Directory.Exists(fileSaveLocation))
                Directory.CreateDirectory(fileSaveLocation);
            CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation);
            List<string> files = new List<string>();
            try
            {
                foreach (MultipartFileData file in provider.FileData)
                {
                    files.Add(Path.GetFileName(file.LocalFileName));
                }
                await Request.Content.ReadAsMultipartAsync(provider);
                // Send OK Response along with saved file names to the client.
                return Request.CreateResponse(HttpStatusCode.OK, files);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage output()
        {
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/upload");
            var filePath = fileSaveLocation + "\\2352885_m.jpg";
            if (File.Exists(filePath))
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(new FileStream(filePath, FileMode.Open, FileAccess.Read));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "2352885_m.jpg"
                };
                return response;
            }
            return ControllerContext.Request.CreateErrorResponse(HttpStatusCode.NotFound, "");
        }
    }
}
