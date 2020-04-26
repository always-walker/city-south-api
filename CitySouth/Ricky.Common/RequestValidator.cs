using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Util;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
/**
 * ============================================================================
 * * 版权所有 2017-2030 we-ai中国，并保留所有权利。
 * 网站地址: http://www.we-ai5.com；
 * ----------------------------------------------------------------------------
 * 这不是一个自由软件！您只能在不用于商业目的的前提下对程序代码进行修改和
 * 使用；不允许对程序代码以任何形式任何目的的再发布。
 * ============================================================================
 * @author:     ricky.gz <zgz521@foxmail.com>
 * @version:    v1.0
 * ---------------------------------------------
 */
namespace Ricky
{
    public class RequestValidator : System.Web.Util.RequestValidator
    {
        protected override bool IsValidRequestString(HttpContext context, string value, RequestValidationSource requestValidationSource, string collectionKey, out int validationFailureIndex)
        {
            validationFailureIndex = -1;
            if (requestValidationSource == RequestValidationSource.Form || requestValidationSource == RequestValidationSource.QueryString)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory.ToString() + "VisitLog\\" + Common.NowDate.ToString("yyyyMMdd");
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
                if (!di.Exists)
                    di.Create();
                string logfile = context.Request.UserHostAddress + ".log";
                try
                {
                    FileInfo fi = new FileInfo(Path.Combine(path, logfile));
                    if (fi.Exists && fi.Length > 4194304) 
                    {
                        return false;
                    }
                    File.AppendAllText(Path.Combine(path, logfile), Common.NowDate.ToString("yyyy-MM-dd HH:mm:ss: ") + string.Format("key:{0},value:{1},url:{2}", collectionKey, value, context.Request.Url) + "\r\n", Encoding.UTF8);
                }
                catch (Exception) { }
            }
            if ((requestValidationSource == RequestValidationSource.Form || requestValidationSource == RequestValidationSource.QueryString) &&
                (Regex.IsMatch(value, @"'+|case[ \t\n\r]+|convert[ \t\n\r]+|union[ \t\n\r]+|where[ \t\n\r]+|when[ \t\n\r]+|and[ \t\n\r]+|select[ \t\n\r]+|insert[ \t\n\r]+|delete[ \t\n\r]+|from[ \t\n\r]+|cast[ \t\n\r]*\(|count[ \t\n\r]*\(|drop[ \t\n\r]+|update[ \t\n\r]+|truncate[ \t\n\r]+|asc[ \t\n\r]*\(|mid[ \t\n\r]*\(|char[ \t\n\r]*\(|chr[ \t\n\r]*\(|xp_cmdshell|exec[ \t\n\r]+|[ \t\n\r]+master|netlocalgroup[ \t\n\r]+|administrator|net[ \t\n\r]+user|<script[\s\S]+</script *>", RegexOptions.IgnoreCase)))
            {
                string path2 = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Exception";
                string str2 = Common.NowDate.ToString("yyyyMMddHH") + ".log";
                try
                {
                    File.AppendAllText(Path.Combine(path2, str2), Common.NowDate.ToString("yyyy-MM-dd HH:mm:ss: ") + string.Format("ip:{2},key:{0},value:{1},url:{3}", collectionKey, value, context.Request.UserHostAddress, context.Request.Url) + "\r\n", Encoding.UTF8);
                }
                catch (Exception) { }
                return false;
            }
            string allowUrl = ConfigurationManager.AppSettings["AllowHtmlUrl"];
            if (!string.IsNullOrEmpty(allowUrl))
            {
                string[] allowUrls = allowUrl.Split(',');
                if (requestValidationSource == RequestValidationSource.Form) //对查询字符串进行验证
                {
                    if (IsAllow(allowUrls, HttpContext.Current.Request.RawUrl))//检查是否包含<,当然也可以检查其他特殊符号,或者忽略某些特殊符号.
                    {
                        return true;
                    }
                }
            }
            return base.IsValidRequestString(context, value, requestValidationSource, collectionKey, out validationFailureIndex);
        }
        private static bool IsAllow(string[] allowUrls, string url)
        {
            foreach (string str in allowUrls)
            {
                if (url.ToLower().Contains(str.ToLower()))
                    return true;
            }
            return false;
        }
    }
}
