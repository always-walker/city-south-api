using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace Ricky.Pay
{
    public class huarongPay : Pay
    {
        public huarongPay() { }
        public huarongPay(string merchantNo, string merchantKey, string actionUrl) : base(merchantNo, merchantKey, actionUrl) { }
        public huarongPay(string merchantNo, string merchantKey, string actionUrl, string pfxFileName, string password) : base(pfxFileName, password, actionUrl, pfxFileName, password) { }
        public override string FormString(string OrderNo, decimal Amount, string frontUrl, string backUrl, string BankCode, string userId, string subject, string body)
        {
            DateTime now = Common.NowDate;
            Dictionary<string, string> dic = new Dictionary<string, string>(){
                    { "requestNo",OrderNo},
                    { "version","V1.5"},
                    { "productId","0208"},//0208网银B2C----0107-支付宝扫码支付0108-微信扫码支付0109-微信wap支付
                    { "transId","12"},//12网银B2C，17扫码支付
                    { "merNo",MerchantNo},
                    { "orderDate",Common.NowDate.ToString("yyyyMMdd")},
                    { "orderNo",OrderNo},
                    { "returnUrl", frontUrl},
                    { "notifyUrl",backUrl},
                    { "transAmt",((int)(Amount*100)).ToString()},
                    { "commodityName",userId},
                    { "bankCode",BankCode}
                 };
            dic.Add("signature", SignatureFormatter(GetSignStr(dic)));
            string postForm = "<form name=\"pay\" id=\"frm1\" method=\"post\" action=\"" + ActionUrl + "\">";
            foreach (var item in dic)
                postForm += "<input type=\"hidden\" name=\"" + item.Key + "\" value=\"" + item.Value + "\" />";
            postForm += "</form>";
            //自动提交该表单到测试网关
            postForm += "<script type=\"text/javascript\" language=\"javascript\">setTimeout(\"document.getElementById('frm1').submit();\",100);</script>";
            string html = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd""><html xmlns=""http://www.w3.org/1999/xhtml"" ><head><meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8""/></head><body>" + postForm + @"</body></html>";
            return html;
        }
        private string getSignMsg(Dictionary<string, string> param)
		{
			List<string> msgList = new List<string>();
			foreach (string key in param.Keys)
			{
                msgList.Add(key + "=" + param[key]);
			}
			// 排序
			msgList.Sort();
			string result = "";
			foreach (string val in msgList)
			{
				result += val + "&";
			}
			// 去掉末尾&
            return result.Substring(0, result.Length - 1);
		}
        public override bool VerifyCallback(Dictionary<string, string> param)
        {

            if (!param.ContainsKey("signature"))
                return false;
            OrderSn = param["orderNo"];
            if (param.ContainsKey("respDesc"))
                Message = param["respDesc"];
            string signature = param["signature"];
            // 删除签名
            param.Remove("signature");
            string newSignature = SignatureFormatter(GetSignStr(param));
            return newSignature.Equals(signature) && param["respCode"] == "0000";
        }
        public override string WechatImgUrl(string OrderNo, decimal Amount, string backUrl, string BankCode, string subject, string body)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>(){
                    { "requestNo",OrderNo},
                    { "version","V1.5"},
                    { "productId","0108"},//0208网银B2C----0107-支付宝扫码支付0108-微信扫码支付0109-微信wap支付
                    { "transId","17"},//12网银B2C，17扫码支付
                    { "merNo",MerchantNo},
                    { "orderDate",Common.NowDate.ToString("yyyyMMdd")},
                    { "orderNo",OrderNo},
                    { "returnUrl",backUrl},
                    { "notifyUrl",backUrl},
                    { "transAmt",((int)(Amount*100)).ToString()},
                    { "commodityName","1"}
                 };
            dic.Add("signature", SignatureFormatter(GetSignStr(dic)));
            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            using (var http = new HttpClient(handler))
            {
                FormUrlEncodedContent content = new FormUrlEncodedContent(dic);
                try
                {
                    Task<byte[]> result = http.PostAsync(ActionUrl, content).Result.Content.ReadAsByteArrayAsync();
                    String resultMsg = System.Text.Encoding.GetEncoding("UTF-8").GetString(result.Result);
                    foreach (string para in resultMsg.Split('&'))
                    {
                        dic2.Add(para.Split('=')[0], para.Substring(para.Split('=')[0].Length + 1));
                    }
                }
                catch { }
            }
            if (dic2.ContainsKey("imgUrl") && !string.IsNullOrEmpty(dic2["imgUrl"]))
                return dic2["imgUrl"];
            else
                return null;
        }
        public override string AlipayImgUrl(string OrderNo, decimal Amount, string backUrl, string BankCode, string subject, string body)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>(){
                    { "requestNo",OrderNo},
                    { "version","V1.5"},
                    { "productId","0107"},//0208网银B2C----0107-支付宝扫码支付0108-微信扫码支付0109-微信wap支付
                    { "transId","17"},//12网银B2C，17扫码支付
                    { "merNo",MerchantNo},
                    { "orderDate",Common.NowDate.ToString("yyyyMMdd")},
                    { "orderNo",OrderNo},
                    { "returnUrl",backUrl},
                    { "notifyUrl",backUrl},
                    { "transAmt",((int)(Amount*100)).ToString()},
                    { "commodityName","1"},
                    { "storeId","123"},
                    { "terminalId","123"}
                 };
            dic.Add("signature", SignatureFormatter(GetSignStr(dic)));
            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            using (var http = new HttpClient(handler))
            {
                FormUrlEncodedContent content = new FormUrlEncodedContent(dic);
                try
                {
                    Task<byte[]> result = http.PostAsync(ActionUrl, content).Result.Content.ReadAsByteArrayAsync();
                    String resultMsg = System.Text.Encoding.GetEncoding("UTF-8").GetString(result.Result);
                    foreach (string para in resultMsg.Split('&'))
                    {
                        dic2.Add(para.Split('=')[0], para.Substring(para.Split('=')[0].Length + 1));
                    }
                }
                catch { }
            }
            if (dic2.ContainsKey("imgUrl") && !string.IsNullOrEmpty(dic2["imgUrl"]))
                return dic2["imgUrl"];
            else
                return null;
        }
    }
}