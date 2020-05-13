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
        [HttpPost]
        [Author("water-electricity.import")]
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
                                    WaterAndElectricity waterE = new WaterAndElectricity();
                                    waterE.OwnerId = owner.OwnerId;
                                    waterE.PayerName = owner.CheckInName;
                                    waterE.UnitPrice = decimal.Parse(dr[5].ToString());
                                    waterE.FeeName = dr[2].ToString();
                                    waterE.FeeType = waterE.FeeName.Equals("水费") ? "water" : waterE.FeeName.Equals("电费") ? "electricity" : "";
                                    if (!string.IsNullOrEmpty(waterE.FeeType))
                                    {
                                        CostConfig config = db.CostConfigs.FirstOrDefault(w => w.EstateId == estate.EstateId && w.ConfigType == waterE.FeeType && w.UnitPrice == waterE.UnitPrice);
                                        if (config != null)
                                            waterE.ConfigId = config.ConfigId;
                                        waterE.CreateTime = DateTime.Now;
                                        DateTime PayTime = DateTime.MinValue;
                                        if (DateTime.TryParse(dr[3].ToString(), out PayTime))
                                            waterE.PayTime = PayTime;
                                        DateTime FeeDate = DateTime.MinValue;
                                        if (DateTime.TryParse(dr[4].ToString(), out PayTime))
                                            waterE.FeeDate = PayTime;
                                        waterE.UnitName = dr[6].ToString();
                                        waterE.LastQuantity = double.Parse(dr[7].ToString());
                                        waterE.Quantity = double.Parse(dr[8].ToString());
                                        waterE.Amount = decimal.Parse(dr[9].ToString());
                                        waterE.ReceiptNo = dr[10].ToString();
                                        waterE.VoucherNo = dr[11].ToString();
                                        waterE.OprationName = dr[12].ToString();
                                        waterE.PayWay = dr[13].ToString();
                                        waterE.Remark = dr[14].ToString();
                                        waterE.Status = dr[15].ToString().Equals("是") ? 1 : 0;
                                        if (db.WaterAndElectricities.Count(w => w.OwnerId == owner.OwnerId && w.LastQuantity == waterE.LastQuantity && w.Quantity == waterE.Quantity && w.FeeDate == waterE.FeeDate) == 0)
                                        {
                                            db.WaterAndElectricities.Add(waterE);
                                            db.SaveChanges();
                                            dr[16] = "成功";
                                            dr[17] = "新增";
                                        }
                                        else
                                        {
                                            WaterAndElectricity newWateE = db.WaterAndElectricities.First(w => w.OwnerId == owner.OwnerId && w.LastQuantity == waterE.LastQuantity && w.Quantity == waterE.Quantity && w.FeeDate == waterE.FeeDate);
                                            Ricky.ObjectCopy.Copy<WaterAndElectricity>(waterE, newWateE, new string[] { "FeeId", "PayerName", "UserId" });
                                            db.SaveChanges();
                                            dr[16] = "成功";
                                            dr[17] = "修改";
                                        }
                                    }
                                    else
                                    {
                                        dr[16] = "失败";
                                        dr[17] = "无法识别费用类型";
                                    }
                                }
                                catch (Exception e)
                                {
                                    dr[16] = "失败";
                                    dr[17] = e.Message;
                                }
                            }
                            else
                            {
                                dr[16] = "失败";
                                dr[17] = "没有找到业主";
                            }
                        }
                        else
                        {
                            dr[16] = "失败";
                            dr[17] = "沒有找到此房产";
                        }
                    }
                    else
                    {
                        dr[16] = "失败";
                        dr[17] = "沒有找到此小区";
                    }
                }
                else
                {
                    dr[16] = "失败";
                    dr[17] = "缺少所属小区名称";
                }
            }
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/upload/water-electricity");
            if (!Directory.Exists(fileSaveLocation))
                Directory.CreateDirectory(fileSaveLocation);
            string Name = fileName.Substring(0, fileName.LastIndexOf('.'));
            Excel excel = new Excel(dt);
            string saveFileName = string.Format("{1}{2}.xlsx", fileSaveLocation, Name, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            excel.Save(fileSaveLocation + "\\" + saveFileName);
            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, filename = saveFileName });
        }
        [HttpGet]
        [Author("water-electricity.import")]
        public HttpResponseMessage ImportFile([FromUri]string filename)
        {
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/upload/water-electricity");
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
