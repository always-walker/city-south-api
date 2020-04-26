using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CitySouth.Data.Models;
using CitySouth.Data;

namespace CitySouth.Web.Controllers
{
    public class EmployeeController : BaseController
    {
        private void setPostIds(List<int> PostIds, int ParentPostId)
        {
            List<Post> posts = db.Posts.Where(w => w.ParentPostId == ParentPostId).ToList();
            foreach(Post post in posts)
            {
                if (post.PostType == "depart")
                    setPostIds(PostIds, post.PostId);
                else
                    PostIds.Add(post.PostId);
            }
        }
        [HttpPost]
        [Author("employee.list")]
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
            var a = from b in db.Employees select b;
            if (PostIds.Count > 0)
                a = a.Where(w => PostIds.Contains(w.PostId));
            if(model.Fk2Id != null)
                a = a.Where(w => w.EstateId == model.Fk2Id);
            else if (!user.IsSuper)
                a = a.Where(w => user.EstateIds.Contains(w.EstateId));
            if (!string.IsNullOrEmpty(model.KeyWord))
                a = a.Where(w => w.EmployeeNo.Contains(model.KeyWord) || w.EmployeeName.Contains(model.KeyWord) || w.Phone.Contains(model.KeyWord) || w.CardId.Contains(model.KeyWord));
            result["count"] = a.Count();
            var list = a.ToList();
            result["datalist"] = list;
            return result;
        }
        [HttpGet]
        [Author("employee.manage")]
        public Dictionary<string, object> PostHistory(int id)
        {
            result["datalist"] = db.EmployeePostHistories.Where(w => w.EmployeeId == id).OrderByDescending(w => w.ChangeDate).ToList();
            return result;
        }
        [HttpPost]
        [Author("employee.manage")]
        public Dictionary<string, object> Add([FromBody]Employee employee)
        {
            if (string.IsNullOrEmpty(employee.EmployeeNo))
            {
                result["code"] = "failed";
                message.Add("员工编号不能为空");
            }
            else if (string.IsNullOrEmpty(employee.EmployeeName))
            {
                result["code"] = "failed";
                message.Add("员工姓名不能为空");
            }
            else if (employee.EntryDate == DateTime.MinValue)
            {
                result["code"] = "failed";
                message.Add("请选择入职时间");
            }
            else if (db.Employees.Count(w => w.EmployeeNo == employee.EmployeeNo) > 0)
            {
                result["code"] = "failed";
                message.Add("员工编号已存在");
            }
            else if (db.Posts.Count(w => w.PostId == employee.PostId && w.PostType == "post") == 0)
            {
                result["code"] = "failed";
                message.Add("请选择岗位");
            }
            else
            {
                db.Employees.Add(employee);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpPut]
        [Author("employee.manage")]
        public Dictionary<string, object> Modify([FromBody]Employee employee)
        {
            if (string.IsNullOrEmpty(employee.EmployeeNo))
            {
                result["code"] = "failed";
                message.Add("员工编号不能为空");
            }
            else if (string.IsNullOrEmpty(employee.EmployeeName))
            {
                result["code"] = "failed";
                message.Add("员工姓名不能为空");
            }
            else if (employee.EntryDate == DateTime.MinValue)
            {
                result["code"] = "failed";
                message.Add("请选择入职时间");
            }
            else if (db.Posts.Count(w => w.PostId == employee.PostId && w.PostType == "post") == 0)
            {
                result["code"] = "failed";
                message.Add("请选择岗位");
            }
            else
            {
                Employee newEmployee = db.Employees.FirstOrDefault(w => w.EmployeeId == employee.EmployeeId);
                if (newEmployee.EmployeeNo != employee.EmployeeNo && db.Employees.Count(w => w.EmployeeNo == employee.EmployeeNo) > 0)
                {
                    result["code"] = "failed";
                    message.Add("员工编号已存在");
                }
                else
                {
                    if (newEmployee.PostId != employee.PostId)
                    {
                        EmployeePostHistory history = new EmployeePostHistory();
                        history.EmployeeId = newEmployee.EmployeeId;
                        history.CurrentPostId = newEmployee.PostId;
                        history.ChangePostId = employee.PostId;
                        history.CreateTime = DateTime.Now;
                        history.UserId = user.UserId;
                        history.Transactor = user.UserName;
                        history.Remark = employee.ChangePostRemark;
                        history.ChangeDate = employee.ChangePostDate ?? DateTime.Now;
                        db.EmployeePostHistories.Add(history);
                    }
                    Ricky.ObjectCopy.Copy<Employee>(employee, newEmployee);
                    db.SaveChanges();
                }
            }
            result["message"] = message;
            return result;
        }
        [HttpDelete]
        [Author("employee.manage")]
        public Dictionary<string, object> Delete(int id)
        {
            Employee employee = db.Employees.FirstOrDefault(w => w.EmployeeId == id);
            db.Employees.Remove(employee);
            db.SaveChanges();
            result["message"] = message;
            return result;
        }
    }
}
