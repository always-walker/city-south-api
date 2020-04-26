using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
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
namespace Ricky.Sms
{
    public class alidy:SMS
    {
        private bool SendMsg(string phone, string content, string template)
        {
            url = "http://gw.api.taobao.com/router/rest";
            WebHeaderCollection headers = new WebHeaderCollection();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("method", "alibaba.aliqin.fc.sms.num.send");
            parameters.Add("app_key", appkey);
            parameters.Add("sign_method", "md5");
            parameters.Add("timestamp", Common.NowDate.ToString("yyyy-MM-dd HH:mm:ss"));
            parameters.Add("format", "json");
            parameters.Add("v", "2.0");
            parameters.Add("sms_type", "normal");
            parameters.Add("sms_free_sign_name", signname);
            parameters.Add("rec_num", phone);
            parameters.Add("sms_template_code", "SMS_56550090");
            parameters.Add("sms_param", content);
            parameters.Add("sign", SignTopRequest(parameters, appsecret, "md5"));
            string bodys = "";
            foreach (string key in parameters.Keys)
            {
                bodys += string.Format("{0}={1}&", key, parameters[key]);
            }
            bodys = bodys.TrimEnd('&');
            method = "post";
            string response = Request("", bodys, headers);
            JObject result = JObject.Parse(response);
            if (result["alibaba_aliqin_fc_sms_num_send_response"] == null)
            {
                errmsg = result["error_response"]["msg"].ToString();
                return false;
            }
            else
                return true;
        }
        public override bool Send(string phone, string content)
        {
            return SendMsg(phone, content, null);
        }
        public override bool Send(string phone, string content, string template)
        {
            return SendMsg(phone, content, template);
        }
        private string SignTopRequest(IDictionary<string, string> parameters, string secret, string signMethod)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters, StringComparer.Ordinal);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder();
            if ("md5".Equals(signMethod))
            {
                query.Append(secret);
            }
            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    query.Append(key).Append(value);
                }
            }

            // 第三步：使用MD5/HMAC加密
            byte[] bytes;
            if ("md5".Equals(signMethod))
            {
                HMACMD5 hmac = new HMACMD5(Encoding.UTF8.GetBytes(secret));
                bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(query.ToString()));
            }
            else
            {
                query.Append(secret);
                MD5 md5 = MD5.Create();
                bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(query.ToString()));
            }

            // 第四步：把二进制转化为大写的十六进制
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                result.Append(bytes[i].ToString("X2"));
            }
            return result.ToString();
        }
    }
}
