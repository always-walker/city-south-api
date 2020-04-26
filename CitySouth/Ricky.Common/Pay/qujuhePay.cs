using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Ricky.Pay
{
    public class qujuhePay : Pay
    {
        public qujuhePay() { }
        public qujuhePay(string merchantNo, string merchantKey, string actionUrl) : base(merchantNo, merchantKey, actionUrl) { }
        public qujuhePay(string merchantNo, string merchantKey, string actionUrl, string pfxFileName, string password) : base(pfxFileName, password, actionUrl, pfxFileName, password) { }
        public override string FormString(string OrderNo, decimal Amount, string frontUrl, string backUrl, string BankCode, string userId, string subject, string body)
        {
            string parms = "pay_amount=" + Amount.ToString("f2")
                           + "&pay_applydate=" + Common.NowDate.Date.ToString()
                           + "&pay_bankcode=" + BankCode
                           + "&pay_callbackurl=" + frontUrl
                           + "&pay_memberid=" + MerchantNo
                           + "&pay_notifyurl=" + backUrl
                           + "&pay_orderid=" + OrderNo
                           + "&key=" + MerchantKey;

            string sParmsMd5 = md5(parms, encoding).ToUpper();
            parms = parms + "&pay_md5sign=" + sParmsMd5;
            parms = parms + "&pay_deviceIp=" + HttpContext.Current.Request.UserHostAddress;
            parms = parms + "&pay_productname=" + subject;
            method = "post";
            url = ActionUrl;
            string sResult = Request("", parms, null);
            JObject jResult = JObject.Parse(sResult);
            if (jResult["status"].ToString() == "success")
            {
                //H5支付请将获取到的mwUrl 使用urlencode
                //H5支付部分 BEGIN
                string postForm = "正在跳转支付...<script type=\"text/javascript\" language=\"javascript\">window.location='" + jResult["data"]["mwUrlWithCallback"].ToString() + "';</script>";
                string html = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd""><html xmlns=""http://www.w3.org/1999/xhtml"" ><head><meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8""/></head><body>" + postForm + @"</body></html>";
                return html;
            }
            throw new NotImplementedException();
        }

        public override bool VerifyCallback(Dictionary<string, string> param)
        {
            logstr(GetSignStr(param));
            string stringSignTemp = "amount=" + param["amount"] + "&datetime=" + param["datetime"] + "&memberid=" + param["memberid"]
                + "&orderid=" + param["orderid"] + "&returncode=" + param["returncode"] + "&transaction_id=" + param["transaction_id"] + "&key=" + MerchantKey;
            string md5Sign = md5(stringSignTemp, Encoding.UTF8).ToUpper();
            OrderSn = param["orderid"];
            return param["sign"].Equals(md5Sign) && param["returncode"].Equals("00");
        }

        public override string WechatImgUrl(string OrderNo, decimal Amount, string backUrl, string BankCode, string subject, string body)
        {
            string parms = "pay_amount=" + Amount.ToString("f2")
                           + "&pay_applydate=" + Common.NowDate.Date.ToString()
                           + "&pay_bankcode=" + "902"
                           + "&pay_callbackurl=" + backUrl
                           + "&pay_memberid=" + MerchantNo
                           + "&pay_notifyurl=" + backUrl
                           + "&pay_orderid=" + OrderNo
                           + "&key=" + MerchantKey;

            string sParmsMd5 = md5(parms, encoding).ToUpper();
            parms = parms + "&pay_md5sign=" + sParmsMd5;
            parms = parms + "&pay_deviceIp=" + HttpContext.Current.Request.UserHostAddress;
            parms = parms + "&pay_productname=" + subject;
            method = "post";
            url = ActionUrl;
            string sResult = Request("", parms, null);
            JObject jResult = JObject.Parse(sResult);
            if (jResult["status"].ToString() == "success")
            {
                return jResult["data"]["code_url"].ToString();
            }
            else
            {
                return null;
            }
        }

        public override string AlipayImgUrl(string OrderNo, decimal Amount, string backUrl, string BankCode, string subject, string body)
        {
            throw new NotImplementedException();
        }
    }
}
