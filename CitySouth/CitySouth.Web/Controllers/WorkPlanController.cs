using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CitySouth.Data.Models;
using CitySouth.Data;
using System.Data.Entity.SqlServer;

namespace CitySouth.Web.Controllers
{
    public class WorkPlanController : BaseController
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
        [HttpPost]
        [Author("work-plan.list")]
        public Dictionary<string, object> Index([FromBody]SearchModel model)
        {
            if (model.SearchDate == null)
                model.SearchDate = DateTime.Now;
            List<int> PostIds = new List<int>();
            if (model.FkId != null && model.FkId > 0)
            {
                Post post = db.Posts.FirstOrDefault(w => w.PostId == model.FkId);
                if (post.PostType == "depart")
                    setPostIds(PostIds, post.PostId);
                else
                    PostIds.Add(post.PostId);
            }
            var a = from b in db.Employees where b.QuitDate == null select b;
            if (PostIds.Count > 0)
                a = a.Where(w => PostIds.Contains(w.PostId));
            if (model.Fk2Id != null)
                a = a.Where(w => w.EstateId == model.Fk2Id);
            else if (!user.IsSuper)
                a = a.Where(w => user.EstateIds.Contains(w.EstateId));
            var employee = from b in a select new { b.EmployeeId, b.EmployeeName, b.PostId, b.EstateId };

            //var plan = (from x in db.WorkPlans where SqlFunctions.DateDiff("MM", model.SearchDate, x.WorkDate) == 0 select x).ToList();
            //var planTimes = (from x in db.WorkPlanTimes where (from p in plan select p.PlanId).Contains(x.PlanId) select x).ToList();

            var planTimes = (from d in db.WorkPlans
                             join t in db.WorkPlanTimes on d.PlanId equals t.PlanId
                             where SqlFunctions.DateDiff("MM", model.SearchDate, d.WorkDate) == 0
                             select new { d.EmployeeId, d.WorkDate, t.ConfigId, t.IsWork }).ToList();

            List<WorkTimeConfig> TimeConfig = db.WorkTimeConfigs.Where(w => w.IsAble == true).OrderBy(w => w.TimeStart).ToList();
            List<MonthPlan> list = new List<MonthPlan>();
            DateTime start = DateTime.Parse(model.SearchDate.Value.ToString("yyyy-MM") + "-01");
            DateTime end = start.AddMonths(1).AddDays(-1);
            int length = end.Day - start.Day + 1;
            int count = TimeConfig.Count;
            foreach (var item in a)
            {
                MonthPlan emplan = new MonthPlan(item.EmployeeId, item.EmployeeName, item.PostId, item.EstateId);
                emplan.Works = new Boolean[length];
                emplan.WorkTimes = new Boolean[length, count];
                for (int i = 0; i < emplan.Works.Length; i++)
                {
                    int day = i + 1;
                    for (int j = 0; j < count; j++)
                    {
                        int ConfigId = TimeConfig[j].ConfigId;
                        if (planTimes.Count(w => w.EmployeeId == emplan.EmployeeId && w.WorkDate.Day == day && w.ConfigId == ConfigId && w.IsWork == true) > 0)
                        {
                            emplan.WorkTimes[i, j] = true;
                            if (model.Fk3Id == null)
                                emplan.Works[i] = true;
                            else if (model.Fk3Id.Value == ConfigId)
                                emplan.Works[i] = true;
                        }

                    }
                    //if (plan.Count(w => w.EmployeeId == emplan.EmployeeId && w.WorkDate.Day == day && w.IsWork == true) > 0)
                    //    emplan.Works[i] = true;
                }
                list.Add(emplan);
            }
            int[] days = new int[length];
            for (int i = 0; i < length; i++)
                days[i] = i + 1;
            result["days"] = days;
            result["times"] = TimeConfig;
            result["datalist"] = list;
            return result;
        } 
        [HttpPost]
        [Author("work-plan.manage")]
        public Dictionary<string, object> Set([FromBody]WorkPlan model)
        {
            WorkPlan plan = db.WorkPlans.FirstOrDefault(w => w.EmployeeId == model.EmployeeId && SqlFunctions.DateDiff("dd", w.WorkDate, model.WorkDate) == 0);
            if (plan == null)
            {
                db.WorkPlans.Add(model);
                db.SaveChanges();
                WorkPlanTime planTime = new WorkPlanTime();
                planTime.PlanId = model.PlanId;
                planTime.ConfigId = model.ConfigId;
                planTime.IsWork = model.IsWork;
                db.WorkPlanTimes.Add(planTime);
                db.SaveChanges();
            }
            else
            {
                WorkPlanTime planTime = db.WorkPlanTimes.FirstOrDefault(w => w.PlanId == plan.PlanId && w.ConfigId == model.ConfigId);
                if (planTime == null)
                {
                    planTime = new WorkPlanTime();
                    planTime.PlanId = plan.PlanId;
                    planTime.ConfigId = model.ConfigId;
                    planTime.IsWork = model.IsWork;
                    db.WorkPlanTimes.Add(planTime);
                    db.SaveChanges();
                }
                else
                    planTime.IsWork = model.IsWork;
                if (db.WorkPlanTimes.Count(w => w.PlanId == plan.PlanId && w.IsWork == true) > 0)
                    plan.IsWork = true;
                else
                    plan.IsWork = false;
                db.SaveChanges();
            }
            return result;
        }
    }
}