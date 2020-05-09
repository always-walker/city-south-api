using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CitySouth.Data.Models;
using CitySouth.Data;
using System.Web;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace CitySouth.Web.Controllers
{
    public class HouseController : BaseController
    {
        [HttpPost]
        [Author("house.list")]
        public Dictionary<string, object> Index([FromBody]SearchModel model)
        {
            var a = from b in db.Houses select b;
            if (model.FkId != null && model.FkId > 0)
                a = a.Where(w => w.EstateId == model.FkId);
            else if(!user.IsSuper)
                a = a.Where(w => user.EstateIds.Contains(w.EstateId));
            if (!string.IsNullOrEmpty(model.KeyWord))
                a = a.Where(w => w.HouseNo.Contains(model.KeyWord) || w.Structure.Contains(model.KeyWord) || w.Model.Contains(model.KeyWord) || w.HouseType.Contains(model.KeyWord) || w.ContactTel.Contains(model.KeyWord));
            if (model.IsCondition1 != null)
                a = a.Where(w => w.IsPlace == model.IsCondition1.Value);
            if (model.StartDate != null)
                a = a.Where(w => w.HandDate >= model.StartDate.Value);
            if (model.EndDate != null)
            {
                model.EndDate = model.EndDate.Value.AddDays(1);
                a = a.Where(w => w.HandDate < model.EndDate.Value);
            }
            result["count"] = a.Count();
            var house = a.OrderBy(w => w.EstateId).ThenBy(w => w.Building).ThenBy(w => w.Unit).ThenBy(w => w.Floor).ThenBy(w => w.No).Skip((model.PageIndex - 1) * model.PageSize).Take(model.PageSize).ToList();
            var estate = db.Estates.ToList();
            var list = from x in house
                       join y in estate on x.EstateId equals y.EstateId
                       select new { house = x, y.EstateName };
            result["datalist"] = list;
            return result;
        }
        [HttpPost]
        [Author("house.manage")]
        public Dictionary<string, object> Batch([FromBody]HouseBatch  batch)
        {
            if (ModelState.IsValid)
            {
                if (batch.ModelList.Count == 0)
                {
                    result["code"] = "failed";
                    message.Add("至少输入一个房号");
                }
                else if (batch.EndBuilding <= batch.StartBuilding)
                {
                    result["code"] = "failed";
                    message.Add("结束栋数必须大于起始栋数");
                }
                else if (batch.EndUnit <= batch.StartUnit)
                {
                    result["code"] = "failed";
                    message.Add("结束单元必须大于起始单元");
                }
                else
                {
                    var a = from b in batch.ModelList group b by b.HouseNo into g select new { no = g.Key, count = g.Count() };
                    if (a.Count(w => w.count > 1) > 0) 
                    {
                        result["code"] = "failed";
                        message.Add("不能有相同的房号");
                    }
                    else if (db.Estates.Count(w => w.EstateId == batch.EstateId) == 0)
                    {
                        result["code"] = "failed";
                        message.Add("选择的小区不存在");
                    }
                    else
                    {
                        //在这里开始批量创建房子
                        for (int i = batch.StartBuilding; i <= batch.EndBuilding; i++)
                        {
                            for (int j = batch.StartUnit; j <= batch.EndUnit; j++)
                            {
                                for (int k = batch.StartFloor; k <= batch.EndFloor; k++)
                                {
                                    foreach (HouseModel m in batch.ModelList)
                                    {
                                        //在这里创建单个房子
                                        House house = new House();
                                        house.EstateId = batch.EstateId;
                                        house.HouseType = batch.HouseType;
                                        house.Building = i;
                                        house.Unit = j;
                                        house.Floor = k;
                                        house.No = m.HouseNo;
                                        house.HouseNo = string.Format("{0}-{1}-{2}-{3}", i, j, k, m.HouseNo);
                                        house.Model = m.Model;
                                        house.Structure = batch.Structure;
                                        house.Floorage = m.Floorage;
                                        house.ContactTel = batch.ContactTel;
                                        house.IsPlace = batch.IsPlace;
                                        house.HandDate = batch.HandDate;
                                        house.EmptyState = batch.EmptyState;
                                        house.News = batch.News;
                                        db.Houses.Add(house);
                                    }
                                }
                            }
                        }
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception e) {
                            result["code"] = "failed";
                            message.Add(e.Message);
                            message.Add("本小区存在房号重复,请查验.");
                        }
                    }
                }
                result["message"] = message;
            }
            else
            {
                result["code"] = "failed";
                message = GetErrorMessage(ModelState.Values);
                if (message.Count == 0)
                    message.Add("参数未填写完整");
                result["message"] = message;
            }
            return result;
        }
        [HttpPost]
        [Author("house.manage")]
        public Dictionary<string, object> Add([FromBody]House house)
        {
            if (string.IsNullOrEmpty(house.HouseNo))
            {
                result["code"] = "failed";
                message.Add("房号不能为空");
            }
            else if (db.Estates.Count(w=>w.EstateId == house.EstateId) == 0)
            {
                result["code"] = "failed";
                message.Add("请选择小区");
            }
            else if (db.Houses.Count(w => w.EstateId == house.EstateId && w.HouseNo == house.HouseNo) > 0)
            {
                result["code"] = "failed";
                message.Add("此房号已存在");
            }
            else
            {
                db.Houses.Add(house);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpPut]
        [Author("house.manage")]
        public Dictionary<string, object> Modify([FromBody]House house)
        {
            if (string.IsNullOrEmpty(house.HouseNo))
            {
                result["code"] = "failed";
                message.Add("房号不能为空");
            }
            else if (db.Estates.Count(w => w.EstateId == house.EstateId) == 0)
            {
                result["code"] = "failed";
                message.Add("请选择小区");
            }
            else
            {
                House newHouse = db.Houses.FirstOrDefault(w => w.HouseId == house.HouseId);
                if (newHouse.HouseNo != house.HouseNo && db.Houses.Count(w => w.EstateId == house.EstateId && w.HouseNo == house.HouseNo) > 0)
                {
                    result["code"] = "failed";
                    message.Add("此房号已存在");
                }
                else
                {
                    Ricky.ObjectCopy.Copy<House>(house, newHouse, new string[] { "IsHasOwner" });
                    db.SaveChanges();
                }
            }
            result["message"] = message;
            return result;
        }
        [HttpDelete]
        [Author("house.manage")]
        public Dictionary<string, object> Delete(int id)
        {
            if (db.Owners.Count(w => w.HouseId == id) > 0)
            {
                result["code"] = "failed";
                message.Add("此房产已有业主，无法删除。");
            }
            else
            {
                House house = db.Houses.FirstOrDefault(w => w.HouseId == id);
                db.Houses.Remove(house);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> import()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/upload/owner");
            if (!Directory.Exists(fileSaveLocation))
                Directory.CreateDirectory(fileSaveLocation);
            CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation);
            List<string> files = new List<string>();
            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);
                foreach (MultipartFileData file in provider.FileData)
                {
                    files.Add(Path.GetFileName(file.LocalFileName));
                }
                var filePath = fileSaveLocation + "\\" + files[0];
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(new FileStream(filePath, FileMode.Open, FileAccess.Read));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = files[0]
                };
                return response;
                
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
