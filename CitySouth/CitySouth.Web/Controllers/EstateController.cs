using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CitySouth.Data.Models;

namespace CitySouth.Web.Controllers
{
    public class EstateController : BaseController
    {
        [HttpGet]
        [Author("estate.list")]
        public Dictionary<string, object> Index()
        {
            result["datalist"] = db.Estates.OrderBy(w => w.EstateId).ToList();
            return result;
        }
        [HttpGet]
        public Dictionary<string, object> Select()
        {

            if (user.IsSuper)
                result["datalist"] = db.Estates.OrderBy(w => w.EstateId).ToList();
            else
                result["datalist"] = db.Estates.Where(w => user.EstateIds.Contains(w.EstateId)).OrderBy(w => w.EstateId).ToList();
            return result;
        }
        [HttpPost]
        [Author("estate.manage")]
        public Dictionary<string, object> Add([FromBody]Estate estate)
        {
            if (string.IsNullOrEmpty(estate.EstateName))
            {
                result["code"] = "failed";
                message.Add("小区名称不能为空");
            }
            else
            {
                db.Estates.Add(estate);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpPut]
        [Author("estate.manage")]
        public Dictionary<string, object> Modify([FromBody]Estate estate)
        {
            if (string.IsNullOrEmpty(estate.EstateName))
            {
                result["code"] = "failed";
                message.Add("小区名称不能为空");
            }
            else
            {
                Estate newEstate = db.Estates.FirstOrDefault(w => w.EstateId == estate.EstateId);
                Ricky.ObjectCopy.Copy<Estate>(estate, newEstate, new string[] { "ImageList", "Introduct" });
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpDelete]
        [Author("estate.manage")]
        public Dictionary<string, object> Delete(int id)
        {
            if (db.Houses.Count(w => w.EstateId == id) > 0)
            {
                result["code"] = "failed";
                message.Add("小区已有房产信息，无法删除。");
            }
            else
            {
                Estate estate = db.Estates.FirstOrDefault(w => w.EstateId == id);
                db.Estates.Remove(estate);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
    }
}
