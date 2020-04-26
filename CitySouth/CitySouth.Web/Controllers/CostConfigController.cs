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
    public class CostConfigController : BaseController
    {
        [HttpGet]
        [Author("cost-config.list")]
        public Dictionary<string, object> Index()
        {
            var list = from b in db.CostConfigs
                       join c in db.Estates on b.EstateId equals c.EstateId
                       orderby b.EstateId, b.ConfigType, b.ConfigId
                       select new { c.EstateName, config = b };
            result["datalist"] = list.ToList();
            return result;
        }
        [HttpPost]
        public Dictionary<string, object> Select([FromBody]SearchModel model)
        {
            string[] types = model.KeyWord.Split(',');
            var list = from b in db.CostConfigs where b.EstateId == model.FkId && types.Contains(b.ConfigType) && b.IsAble == true orderby b.IsDefault descending select b;
            result["datalist"] = list.ToList();
            return result;
        }
        [HttpPost]
        [Author("cost-config.add")]
        public Dictionary<string, object> add([FromBody]CostConfig config)
        {
            if (string.IsNullOrEmpty(config.ConfigType))
            {
                result["code"] = "failed";
                message.Add("配置类型必选");
            }
            else if (string.IsNullOrEmpty(config.ConfigVersion))
            {
                result["code"] = "failed";
                message.Add("配置版本必填");
            }
            else if (string.IsNullOrEmpty(config.Category))
            {
                result["code"] = "failed";
                message.Add("配置分类必填");
            }
            if (result["code"].ToString() == "success")
            {
                config.UserId = user.UserId;
                config.ModifyDate = Ricky.Common.NowDate;
                db.CostConfigs.Add(config);
                db.SaveChanges();
                if (config.IsDefault)
                {
                    var a = from b in db.CostConfigs where b.ConfigType == config.ConfigType && b.EstateId == config.EstateId && b.ConfigId != config.ConfigId select b;
                    foreach (var item in a)
                    {
                        item.IsDefault = false;
                    }
                    db.SaveChanges();
                }
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
        [HttpPut]
        [Author("cost-config.modify")]
        public Dictionary<string, object> modify([FromBody]CostConfig config)
        {
            if (db.Properties.Count(w => w.ConfigId == config.ConfigId) > 0)
            {
                result["code"] = "failed";
                message.Add("此配置已有缴费使用,只能禁用");
            }
            else if (db.Parkings.Count(w => w.ConfigId == config.ConfigId) > 0)
            {
                result["code"] = "failed";
                message.Add("此配置已有缴费使用,只能禁用");
            }
            else if (db.WaterAndElectricities.Count(w => w.ConfigId == config.ConfigId) > 0)
            {
                result["code"] = "failed";
                message.Add("此配置已有缴费使用,只能禁用");
            }
            else if (db.ElseCosts.Count(w => w.ConfigId == config.ConfigId) > 0)
            {
                result["code"] = "failed";
                message.Add("此配置已有缴费使用,只能禁用");
            }
            else if (string.IsNullOrEmpty(config.ConfigType))
            {
                result["code"] = "failed";
                message.Add("配置类型必选");
            }
            else if (string.IsNullOrEmpty(config.ConfigVersion))
            {
                result["code"] = "failed";
                message.Add("配置版本必填");
            }
            else if (string.IsNullOrEmpty(config.Category))
            {
                result["code"] = "failed";
                message.Add("配置分类必填");
            }
            if (result["code"].ToString() == "success")
            {
                config.UserId = user.UserId;
                config.ModifyDate = Ricky.Common.NowDate;
                CostConfig newConfig = db.CostConfigs.FirstOrDefault(w => w.ConfigId == config.ConfigId);
                Ricky.ObjectCopy.Copy<CostConfig>(config, newConfig);
                db.SaveChanges();
                if (config.IsDefault)
                {
                    var a = from b in db.CostConfigs where b.ConfigType == config.ConfigType && b.EstateId == config.EstateId && b.ConfigId != config.ConfigId select b;
                    foreach (var item in a)
                    {
                        item.IsDefault = false;
                    }
                    db.SaveChanges();
                }
            }
            result["message"] = message;
            return result;
        }
        [HttpDelete]
        [Author("cost-config.delete")]
        public Dictionary<string, object> delete(int id)
        {
            if (db.Properties.Count(w => w.ConfigId == id) > 0)
            {
                result["code"] = "failed";
                message.Add("此配置已有缴费使用");
            }
            else if (db.Parkings.Count(w => w.ConfigId == id) > 0)
            {
                result["code"] = "failed";
                message.Add("此配置已有缴费使用");
            }
            else if (db.WaterAndElectricities.Count(w => w.ConfigId == id) > 0)
            {
                result["code"] = "failed";
                message.Add("此配置已有缴费使用");
            }
            else if (db.ElseCosts.Count(w => w.ConfigId == id) > 0)
            {
                result["code"] = "failed";
                message.Add("此配置已有缴费使用");
            }
            if (result["code"].ToString() == "success")
            {
                CostConfig config = db.CostConfigs.FirstOrDefault(w => w.ConfigId == id);
                db.CostConfigs.Remove(config);
                db.SaveChanges();
            }
            result["message"] = message;
            return result;
        }
    }
}
