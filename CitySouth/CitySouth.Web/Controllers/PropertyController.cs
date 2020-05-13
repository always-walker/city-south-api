using CitySouth.Data;
using CitySouth.Data.Models;
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
        [HttpPost]
        [Author("property.import")]
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
                                try
                                {
                                    Property property = new Property();
                                    property.OwnerId = owner.OwnerId;
                                    property.PayerName = owner.CheckInName;
                                    property.UnitPrice = decimal.Parse(dr[2].ToString());
                                    string UnitName = dr[3].ToString();
                                    CostConfig config = db.CostConfigs.FirstOrDefault(w => w.EstateId == estate.EstateId && w.ConfigType == "property" && w.UnitPrice == property.UnitPrice && w.UnitName == UnitName);
                                    if (config != null)
                                        property.ConfigId = config.ConfigId;
                                    property.MonthCount = int.Parse(dr[4].ToString());
                                    property.Amount = decimal.Parse(dr[5].ToString());
                                    property.CreateTime = DateTime.Now;
                                    DateTime PayTime = DateTime.MinValue;
                                    if (DateTime.TryParse(dr[6].ToString(), out PayTime))
                                        property.PayTime = PayTime;
                                    DateTime StartDate = DateTime.MinValue;
                                    if (DateTime.TryParse(dr[7].ToString(), out StartDate))
                                        property.StartDate = StartDate;
                                    DateTime EndDate = DateTime.MinValue;
                                    if (DateTime.TryParse(dr[8].ToString(), out EndDate))
                                        property.EndDate = EndDate;
                                    property.ReceiptNo = dr[9].ToString();
                                    property.VoucherNo = dr[10].ToString();
                                    property.OprationName = dr[11].ToString();
                                    property.PayWay = dr[12].ToString();
                                    property.Remark = dr[13].ToString();
                                    if (property.PayTime != null)
                                        property.Status = 1;
                                    if (db.Properties.Count(w => w.OwnerId == property.OwnerId && w.Amount == property.Amount && w.MonthCount == property.MonthCount && w.StartDate == property.StartDate && w.EndDate == property.EndDate) == 0)
                                    {
                                        db.Properties.Add(property);
                                        db.SaveChanges();
                                        dr[14] = "成功";
                                        dr[15] = "新增";
                                    }
                                    else
                                    {
                                        Property newProperty = db.Properties.First(w => w.OwnerId == property.OwnerId && w.Amount == property.Amount && w.MonthCount == property.MonthCount && w.StartDate == property.StartDate && w.EndDate == property.EndDate);
                                        Ricky.ObjectCopy.Copy<Property>(property, newProperty, new string[] { "PropertyId", "PayerName", "UserId" });
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
            db.Database.ExecuteSqlCommand("update [Owner] set PropertyExpireDate=t.EndDate from (select OwnerId,MAX(EndDate) EndDate from [Property] group by OwnerId)t where [Owner].OwnerId=t.OwnerId and ([Owner].PropertyExpireDate is null or [Owner].PropertyExpireDate<t.EndDate)");
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/upload/property");
            if (!Directory.Exists(fileSaveLocation))
                Directory.CreateDirectory(fileSaveLocation);
            string Name = fileName.Substring(0, fileName.LastIndexOf('.'));
            Excel excel = new Excel(dt);
            string saveFileName = string.Format("{1}{2}.xlsx", fileSaveLocation, Name, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            excel.Save(fileSaveLocation + "\\" + saveFileName);
            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, filename = saveFileName });
        }
        [HttpGet]
        [Author("property.import")]
        public HttpResponseMessage ImportFile([FromUri]string filename)
        {
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/upload/property");
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
    }
}
