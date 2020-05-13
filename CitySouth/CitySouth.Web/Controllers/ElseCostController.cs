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
    public class ElseCostController : BaseController
    {
        [HttpGet]
        public Dictionary<string, object> Group()
        {
            var a = from b in db.ElseCosts group b by b.CostName into g select g.Key;
            result["groups"] = a.ToList();
            return result;
        }
        [HttpPost]
        [Author("else-cost.list")]
        public Dictionary<string, object> Index([FromBody]SearchModel model)
        {
            var a = from elseCost in db.ElseCosts
                    join b in db.Owners on elseCost.OwnerId equals b.OwnerId
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
                        elseCost
                    };
            if (model.FkId != null && model.FkId > 0)
                a = a.Where(w => w.EstateId == model.FkId);
            else if (!user.IsSuper)
                a = a.Where(w => user.EstateIds.Contains(w.EstateId));
            if (!string.IsNullOrEmpty(model.KeyWord))
                a = a.Where(w => w.HouseNo.Contains(model.KeyWord) || w.OwnerName.Contains(model.KeyWord) || w.CheckInName.Contains(model.KeyWord) || w.Phone.Contains(model.KeyWord) || w.elseCost.VoucherNo.Contains(model.KeyWord) || w.elseCost.ReceiptNo.Contains(model.KeyWord) || w.elseCost.Remark.Contains(model.KeyWord));
            if (!string.IsNullOrEmpty(model.type))
                a = a.Where(w => w.elseCost.CostName == model.type);
            if (model.StartDate != null)
                a = a.Where(w => w.elseCost.CreateTime >= model.StartDate.Value);
            if (model.EndDate != null)
            {
                model.EndDate = model.EndDate.Value.AddDays(1);
                a = a.Where(w => w.elseCost.CreateTime < model.EndDate.Value);
            }
            result["count"] = a.Count();
            var list = a.OrderByDescending(w => w.elseCost.ElseCostId).Skip((model.PageIndex - 1) * model.PageSize).Take(model.PageSize).ToList();
            result["datalist"] = list;
            return result;
        }
        [HttpPost]
        [Author("else-cost.manage")]
        public Dictionary<string, object> Add([FromBody]ElseCost elseCost)
        {
            if (elseCost.OwnerId == 0)
            {
                result["code"] = "failed";
                message.Add("必须选择一个业主");
            }
            else
            {
                elseCost.UserId = user.UserId;
                elseCost.OprationName = user.UserName;
                elseCost.CreateTime = Ricky.Common.NowDate;
                if (elseCost.Status == 1)
                {
                    elseCost.PayTime = elseCost.CreateTime;
                    Owner owner = db.Owners.FirstOrDefault(w => w.OwnerId == elseCost.OwnerId);
                    elseCost.PayerName = string.IsNullOrEmpty(owner.CheckInName) ? owner.OwnerName : owner.CheckInName;
                }
                db.ElseCosts.Add(elseCost);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpPut]
        [Author("else-cost.manage")]
        public Dictionary<string, object> Modify([FromBody]ElseCost elseCost)
        {
            ElseCost newElseCost = db.ElseCosts.FirstOrDefault(w => w.ElseCostId == elseCost.ElseCostId);
            if (elseCost.Status > -1)
            {
                newElseCost.ConfigId = elseCost.ConfigId;
                newElseCost.ReceiptNo = elseCost.ReceiptNo;
                newElseCost.VoucherNo = elseCost.VoucherNo;
                newElseCost.PayWay = elseCost.PayWay;
                newElseCost.Remark = elseCost.Remark;
                if (newElseCost.Status == 0)
                {
                    newElseCost.CostName = elseCost.CostName;
                    newElseCost.StartDate = elseCost.StartDate;
                    newElseCost.EndDate = elseCost.EndDate;
                    newElseCost.UnitPrice = elseCost.UnitPrice;
                    newElseCost.Amount = elseCost.Amount;
                }
                if (newElseCost.Status == 0 && elseCost.Status == 1)
                {
                    newElseCost.PayTime = Ricky.Common.NowDate;
                    newElseCost.Status = elseCost.Status;
                    newElseCost.OprationName = user.UserName;
                    Owner owner = db.Owners.FirstOrDefault(w => w.OwnerId == newElseCost.OwnerId);
                    newElseCost.PayerName = string.IsNullOrEmpty(owner.CheckInName) ? owner.OwnerName : owner.CheckInName;
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
        [Author("else-cost.delete")]
        public Dictionary<string, object> Delete(int id)
        {
            ElseCost elseCost = db.ElseCosts.FirstOrDefault(w => w.ElseCostId == id);
            if (elseCost.Status > -1)
            {
                if (elseCost.Status == 1)
                {
                    elseCost.PayTime = null;
                }
                elseCost.Status = -1;
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
        [Author("else-cost.import")]
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
                                    ElseCost elseC = new ElseCost();
                                    elseC.OwnerId = owner.OwnerId;
                                    elseC.PayerName = owner.CheckInName;
                                    elseC.CostName = dr[2].ToString();
                                    elseC.Amount = decimal.Parse(dr[3].ToString());
                                    DateTime PayTime = DateTime.MinValue;
                                    if (DateTime.TryParse(dr[4].ToString(), out PayTime))
                                        elseC.PayTime = PayTime;
                                    DateTime StartDate = DateTime.MinValue;
                                    if (DateTime.TryParse(dr[5].ToString(), out StartDate))
                                        elseC.StartDate = StartDate;
                                    DateTime EndDate = DateTime.MinValue;
                                    if (DateTime.TryParse(dr[6].ToString(), out EndDate))
                                        elseC.EndDate = EndDate;
                                    elseC.ReceiptNo = dr[7].ToString();
                                    elseC.VoucherNo = dr[8].ToString();
                                    elseC.OprationName = dr[9].ToString();
                                    elseC.PayWay = dr[10].ToString();
                                    elseC.Remark = dr[11].ToString();
                                    if (elseC.PayTime != null)
                                    {
                                        elseC.Status = 1;
                                        elseC.CreateTime = elseC.PayTime.Value;
                                    }
                                    else
                                    {
                                        elseC.CreateTime = DateTime.Now;
                                    }
                                    if (db.ElseCosts.Count(w => w.OwnerId == elseC.OwnerId && w.Amount == elseC.Amount && w.PayTime == elseC.PayTime && w.StartDate == elseC.StartDate && w.EndDate == elseC.EndDate) == 0)
                                    {
                                        db.ElseCosts.Add(elseC);
                                        db.SaveChanges();
                                        dr[12] = "成功";
                                        dr[13] = "新增";
                                    }
                                    else
                                    {
                                        ElseCost newElseC = db.ElseCosts.First(w => w.OwnerId == elseC.OwnerId && w.Amount == elseC.Amount && w.PayTime == elseC.PayTime && w.StartDate == elseC.StartDate && w.EndDate == elseC.EndDate);
                                        Ricky.ObjectCopy.Copy<ElseCost>(elseC, newElseC, new string[] { "ElseCostId", "PayerName", "UserId" });
                                        db.SaveChanges();
                                        dr[12] = "成功";
                                        dr[13] = "修改";
                                    }
                                }
                                catch (Exception e)
                                {
                                    dr[12] = "失败";
                                    dr[13] = e.Message;
                                }
                            }
                            else
                            {
                                dr[12] = "失败";
                                dr[13] = "没有找到业主";
                            }
                        }
                        else
                        {
                            dr[12] = "失败";
                            dr[13] = "沒有找到此房产";
                        }
                    }
                    else
                    {
                        dr[12] = "失败";
                        dr[13] = "沒有找到此小区";
                    }
                }
                else
                {
                    dr[12] = "失败";
                    dr[13] = "缺少所属小区名称";
                }
            }
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/upload/else-cost");
            if (!Directory.Exists(fileSaveLocation))
                Directory.CreateDirectory(fileSaveLocation);
            string Name = fileName.Substring(0, fileName.LastIndexOf('.'));
            Excel excel = new Excel(dt);
            string saveFileName = string.Format("{1}{2}.xlsx", fileSaveLocation, Name, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            excel.Save(fileSaveLocation + "\\" + saveFileName);
            return Request.CreateResponse(HttpStatusCode.OK, new { Status = 1, filename = saveFileName });
        }
        [HttpGet]
        [Author("else-cost.import")]
        public HttpResponseMessage ImportFile([FromUri]string filename)
        {
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/upload/else-cost");
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
