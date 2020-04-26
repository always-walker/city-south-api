using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ricky.Pay
{
    public class ainongPay : Pay
    {
        public ainongPay() { }
        public ainongPay(string merchantNo, string merchantKey, string actionUrl) : base(merchantNo, merchantKey, actionUrl) { }
        public ainongPay(string merchantNo, string merchantKey, string actionUrl, string pfxFileName, string password) : base(pfxFileName, password, actionUrl, pfxFileName, password) { }
        public override string FormString(string OrderNo, decimal Amount, string frontUrl, string backUrl, string BankCode, string userId, string subject, string body)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("version", "1.0.0");
			param.Add("txnType", "01");
			param.Add("txnSubType", "01");
			param.Add("bizType", "000000");
			param.Add("accessType", "0");
			param.Add("accessMode", "01");
			param.Add("merId", MerchantNo);
            param.Add("merOrderId", OrderNo);//商户订单号
			param.Add("txnTime", Common.NowDate.ToString("yyyyMMddHHmmss"));
            param.Add("txnAmt", ((int)(Amount * 100)).ToString());//金额1分钱
            param.Add("currency", "CNY");//金额1分钱
            param.Add("frontUrl", frontUrl);//前台通知地址
            param.Add("backUrl", backUrl);//后台通知地址
			param.Add("payType", "0201");//B2C网银
            param.Add("bankId", BankCode);//工行01020000
            //param.Add("userId", userId);//下单会员编号
            param.Add("subject", subject);
            param.Add("body", body);
			param.Add("merResv1", "");
			// 获取签名明文
			string signMsg = getSignMsg(param);
			logstr(signMsg);
			//在签名字符串后面加上商户秘钥
			signMsg = signMsg + MerchantKey;
			//计算签名
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string signature = Convert.ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(signMsg)));
			// 添加签名方法和签名
			param.Add("signMethod", "MD5");
			param.Add("signature", signature);
			//指定字段做base64
			param["subject"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(param["subject"]));
            param["body"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(param["body"]));
            string html = "<html>";
			html += "<head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\"><title>Pay Page</title></head>";
			html += "<body>";
			html += "<form action=\"" + ActionUrl + "\" method=\"POST\" name=\"orderForm\">";
            foreach (string key in param.Keys)
            {
                html += "<input type=\"hidden\" name=\"" + key + "\"  value=\"" + param[key] + "\">";
            }
			html += "<script type=\"text/javascript\">document.orderForm.submit();</script></body></html>";
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
            string signature = param["signature"];
            // 删除签名方法及签名
            param.Remove("signMethod");
            param.Remove("signature");
            OrderSn = param["merOrderId"];
            if (param.ContainsKey("respMsg"))
                Message = param["respMsg"];
            string[] base64Key = new string[] { "subject", "body", "remark", "customerInfo", "accResv", "riskRateInfo", "billpQueryInfo", "billDetailInfo", "respMsg", "resv" };
            for (int i = 0; i < base64Key.Length; i++)
            {
                // base64解码
                if (param.ContainsKey(base64Key[i]))
                {
                    string val = param[base64Key[i]];
                    param[base64Key[i]] = Encoding.UTF8.GetString(Convert.FromBase64String(val));

                }
            }
            // 获取签名明文
            string signMsg = getSignMsg(param);
            logstr(signMsg);
            //在签名字符串后面加上商户秘钥
            signMsg = signMsg + MerchantKey;
            //计算签名
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string newSignature = Convert.ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(signMsg)));
            return newSignature.Equals(signature) && param["respCode"] == "1001";
        }
        public override string WechatImgUrl(string OrderNo, decimal Amount, string backUrl, string BankCode, string subject, string body)
        {
            throw new NotImplementedException();
        }
        public override string AlipayImgUrl(string OrderNo, decimal Amount, string backUrl, string BankCode, string subject, string body)
        {
            throw new NotImplementedException();
        }
    }
}