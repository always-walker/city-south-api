using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using Newtonsoft.Json;

namespace CitySouth.Data.Models
{
    public class LogItem 
    {
        public LogItem(string name, object[] value)
        {
            this.name = name;
            this.value = value;
        }
        public string name { get; set; }
        public object[] value { get; set; }
    }
    public partial class sysUserLog
    {
        public void add(CitySouthContext db) 
        {
            this.Ip = System.Web.HttpContext.Current.Request.UserHostAddress;
            this.LogTime = DateTime.Now;
            db.sysUserLogs.Add(this);
        }
        public void add(CitySouthContext db, sysUser user)
        {
            this.UserId = user.UserId;
            this.LoginName = user.LoginName;
            this.Ip = System.Web.HttpContext.Current.Request.UserHostAddress;
            this.LogTime = DateTime.Now;
            db.sysUserLogs.Add(this);
        }
        public void add(CitySouthContext db, sysUser user, string LogType, string Remark)
        {
            this.LogType = LogType;
            this.Remark = Remark;
            this.UserId = user.UserId;
            this.LoginName = user.LoginName;
            this.Ip = System.Web.HttpContext.Current.Request.UserHostAddress;
            this.LogTime = DateTime.Now;
            db.sysUserLogs.Add(this);
        }
        public void add<T>(CitySouthContext db, sysUser user, string LogType, T t1)
        {
            var dType = typeof(T);
            List<LogItem> items = new List<LogItem>();
            foreach (PropertyInfo dP in dType.GetProperties())
            {
                object value = dType.GetProperty(dP.Name).GetValue(t1, null);
                if (value != null && (dP.PropertyType != typeof(DateTime) || Convert.ToDateTime(value) != DateTime.MinValue))
                {
                    items.Add(new LogItem(dP.Name, new object[] { value }));
                }
            }

            this.LogType = LogType;
            this.Remark = JsonConvert.SerializeObject(items);
            this.UserId = user.UserId;
            this.LoginName = user.LoginName;
            this.Ip = System.Web.HttpContext.Current.Request.UserHostAddress;
            this.LogTime = DateTime.Now;
            db.sysUserLogs.Add(this);
        }
        public void add<T>(CitySouthContext db, sysUser user, string LogType, T t1, T t2)
        {
            var dType = typeof(T);
            List<LogItem> items = new List<LogItem>();
            foreach (PropertyInfo dP in dType.GetProperties())
            {
                object value1 = dType.GetProperty(dP.Name).GetValue(t1, null);
                object value2 = dType.GetProperty(dP.Name).GetValue(t2, null);
                if ((value1 != null || value2 != null) && ((value1 == null && value2 != null) || (value1 != null && value2 == null) || !value1.Equals(value2)))
                {
                    items.Add(new LogItem(dP.Name, new object[] { value1, value2 }));
                }
            }
            if (items.Count > 1)
            {
                this.LogType = LogType;
                this.Remark = JsonConvert.SerializeObject(items);
                this.UserId = user.UserId;
                this.LoginName = user.LoginName;
                this.Ip = System.Web.HttpContext.Current.Request.UserHostAddress;
                this.LogTime = DateTime.Now;
                db.sysUserLogs.Add(this);
            }
        }
    }
}
