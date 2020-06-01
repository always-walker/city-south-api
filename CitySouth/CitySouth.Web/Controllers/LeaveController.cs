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

namespace CitySouth.Web.Controllers
{
    public class LeaveController : BaseController
    {
        private void setPostIds(List<int> PostIds, int ParentPostId)
        {
            List<Post> posts = db.Posts.Where(w => w.ParentPostId == ParentPostId).ToList();
            foreach (Post post in posts)
            {
                if (post.PostType == "depart")
                    setPostIds(PostIds, post.PostId);
                else
                    PostIds.Add(post.PostId);
            }
        }
        [HttpGet]
        public Dictionary<string, object> Group()
        {
            var a = from b in db.ApplyLeaves group b by b.LeaveType into g select g.Key;
            result["groups"] = a.ToList();
            return result;
        }
        [HttpPost]
        [Author("leave.list")]
        public Dictionary<string, object> Index([FromBody]SearchModel model)
        {
            List<int> PostIds = new List<int>();
            if (model.FkId != null && model.FkId > 0)
            {
                Post post = db.Posts.FirstOrDefault(w => w.PostId == model.FkId);
                if (post.PostType == "depart")
                    setPostIds(PostIds, post.PostId);
                else
                    PostIds.Add(post.PostId);
            }
            var a = from b in db.ApplyLeaves
                    join c in db.Employees on b.EmployeeId equals c.EmployeeId
                    select new
                    {
                        c.PostId,
                        c.EmployeeId,
                        c.EmployeeName,
                        c.Phone,
                        c.CardId,
                        c.EstateId,
                        c.EmployeeNo,
                        leave = b
                    };
            if (PostIds.Count > 0)
                a = a.Where(w => PostIds.Contains(w.PostId));
            if (model.Fk2Id != null)
                a = a.Where(w => w.EstateId == model.Fk2Id);
            else if (!user.IsSuper)
                a = a.Where(w => user.EstateIds.Contains(w.EstateId));
            if (!string.IsNullOrEmpty(model.KeyWord))
                a = a.Where(w => w.EmployeeNo.Contains(model.KeyWord) || w.EmployeeName.Contains(model.KeyWord) || w.Phone.Contains(model.KeyWord) || w.CardId.Contains(model.KeyWord));
            if (!string.IsNullOrEmpty(model.type))
                a = a.Where(w => w.leave.LeaveType == model.type);
            result["count"] = a.Count();
            var list = a.OrderByDescending(w => w.leave.LeaveId).Skip((model.PageIndex - 1) * model.PageSize).Take(model.PageSize).ToList();
            result["datalist"] = list;
            return result;
        }
        [HttpPost]
        [Author("leave.manage")]
        public Dictionary<string, object> Add([FromBody]ApplyLeave leave)
        {
            if (leave.EmployeeId == 0)
            {
                result["code"] = "failed";
                message.Add("必须选择请假的员工");
            }
            else if (string.IsNullOrEmpty(leave.LeaveType))
            {
                result["code"] = "failed";
                message.Add("请选择请假类型");
            }
            else if (leave.Days <= 0)
            {
                result["code"] = "failed";
                message.Add("请填写请假天数");
            }
            else if (leave.StartDate == DateTime.MaxValue)
            {
                result["code"] = "failed";
                message.Add("请选择请假开始时间");
            }
            else
            {
                leave.CreateTime = Ricky.Common.NowDate;
                db.ApplyLeaves.Add(leave);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpPut]
        [Author("leave.manage")]
        public Dictionary<string, object> Modify([FromBody]ApplyLeave leave)
        {
            ApplyLeave newLeave = db.ApplyLeaves.FirstOrDefault(w => w.LeaveId == leave.LeaveId);
            if (!leave.IsDelete)
            {
                Ricky.ObjectCopy.Copy<ApplyLeave>(leave, newLeave, new string[] { "EmployeeId", "CreateTime" });
                db.SaveChanges();
            }
            else
            {
                result["code"] = "failed";
                message.Add("此假条已作废");
            }
            result["message"] = message;
            return result;
        }
        [HttpDelete]
        [Author("leave.delete")]
        public Dictionary<string, object> Delete(int id)
        {
            ApplyLeave leave = db.ApplyLeaves.FirstOrDefault(w => w.LeaveId == id);
            if (!leave.IsDelete)
            {
                leave.IsDelete = true;
                db.SaveChanges();
            }
            else
            {
                result["code"] = "failed";
                message.Add("此假条已作废");
            }
            result["message"] = message;
            return result;
        }
        [Author("leave.export")]
        public HttpResponseMessage Export([FromBody]SearchModel model)
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
            var list = from b in a
                       select new
                       {
                           项目名称 = b.EstateName,
                           房号 = b.HouseNo,
                           业主姓名 = b.OwnerName,
                           缴费人姓名 = b.elseCost.PayerName,
                           联系电话 = b.Phone,
                           费用类别 = b.elseCost.CostName,
                           缴费金额 = b.elseCost.Amount,
                           缴费时间 = b.elseCost.CreateTime,
                           支付时间 = b.elseCost.PayTime,
                           服务开始时间 = b.elseCost.StartDate,
                           服务结束时间 = b.elseCost.EndDate,
                           收据号码 = b.elseCost.ReceiptNo,
                           凭证号码 = b.elseCost.VoucherNo,
                           操作员 = b.elseCost.OprationName,
                           缴费方式 = b.elseCost.PayWay,
                           备注 = b.elseCost.Remark
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
                FileName = HttpUtility.UrlEncode("其它费用.xlsx")
            };
            return response;
        }
    }
}
