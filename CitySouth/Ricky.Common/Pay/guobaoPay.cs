using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Ricky.Pay
{
    public class guobaoPay : Pay
    {
        public guobaoPay() { }
        public guobaoPay(string merchantNo, string merchantKey, string actionUrl) : base(merchantNo, merchantKey, actionUrl) { }
        public guobaoPay(string merchantNo, string merchantKey, string actionUrl, string pfxFileName, string password) : base(pfxFileName, password, actionUrl, pfxFileName, password) { }
        public override string FormString(string OrderNo, decimal Amount, string frontUrl, string backUrl, string BankCode, string userId, string subject, string body)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("version", "2.2");
            param.Add("charset", "1");
            param.Add("language", "1");
            param.Add("signType", "1");
            param.Add("tranCode", "8888");
            param.Add("merchantID", MerchantNo);
			param.Add("merId", MerchantNo);
            param.Add("merOrderNum", OrderNo);//商户订单号
            param.Add("tranAmt", Amount.ToString("f2"));//金额
            param.Add("feeAmt", "0");
            param.Add("currencyType", "156");//
            param.Add("frontMerUrl", frontUrl);//前台通知地址
            param.Add("backgroundMerUrl", backUrl);//后台通知地址
            param.Add("tranDateTime", Common.NowDate.ToString("yyyyMMddHHmmss"));
            param.Add("virCardNoIn", Param1);//国宝转入账户
            param.Add("tranIP", HttpContext.Current.Request.UserHostAddress);
			param.Add("payType", "0201");//B2C网银
            param.Add("isRepeatSubmit", "1");
            param.Add("goodsName", subject);
            param.Add("goodsDetail", body);
            param.Add("buyerName", userId);//下单会员编号
            param.Add("buyerContact", "");
            param.Add("merRemark1", "");
            param.Add("merRemark2", "");
            param.Add("bankCode", BankCode);//银行直连代码
            param.Add("userType", "1");//1
            //获取国付宝时间
            url = "https://gateway.gopay.com.cn/time.do";
            param.Add("gopayServerTime", Request("", "", null));
			// 获取签名明文
			string signMsg = getSignMsg(param);
			logstr(signMsg);
			//计算签名
			// 添加签名方法和签名
            param.Add("signValue", md5(signMsg, Encoding.Default));
            string html = "<html>";
            html += "<head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\"><title>Pay Page</title></head>";
            html += "<body>";
            html += "<form action=\"" + (string.IsNullOrEmpty(ChangeUrl) ? ActionUrl : ChangeUrl) + "\" method=\"POST\" name=\"orderForm\">";
            foreach (string key in param.Keys)
            {
                html += "<input type=\"hidden\" name=\"" + key + "\"  value=\"" + param[key] + "\">";
            }
            if (!string.IsNullOrEmpty(ChangeUrl))
            {
                html += "<input type=\"hidden\" name=\"ActionUrl\"  value=\"" + ActionUrl + "\">";
            }
            html += "<script type=\"text/javascript\">document.orderForm.submit();</script></body></html>";
            return html;
        }
        private string getSignMsg(Dictionary<string, string> param)
		{
            string result = "version=[" + param["version"] + "]tranCode=[" + param["tranCode"] + "]merchantID=[" +
                 param["merchantID"] + "]merOrderNum=[" + param["merOrderNum"] + "]tranAmt=[" + param["tranAmt"] + "]feeAmt=[" +
                  param["feeAmt"] + "]tranDateTime=[" + param["tranDateTime"] + "]frontMerUrl=[" + param["frontMerUrl"] +
                  "]backgroundMerUrl=[" + param["backgroundMerUrl"] + "]orderId=[]gopayOutOrderId=[]tranIP=[" + param["tranIP"] +
                  "]respCode=[]gopayServerTime=[" + param["gopayServerTime"] + "]VerficationCode=[" + MerchantKey + "]";
            return result;
		}
        public override bool VerifyCallback(Dictionary<string, string> param)
        {
            if (!param.ContainsKey("signValue"))
                return false;
            string plain = "version=[" + param["version"] + "]tranCode=[" + param["tranCode"] + "]merchantID=[" + param["merchantID"]
                + "]merOrderNum=[" + param["merOrderNum"] + "]tranAmt=[" + param["tranAmt"] + "]feeAmt=[" + param["feeAmt"]
                + "]tranDateTime=[" + param["tranDateTime"] + "]frontMerUrl=[" + param["frontMerUrl"]
                + "]backgroundMerUrl=[" + param["backgroundMerUrl"] + "]orderId=[" + param["orderId"]
                + "]gopayOutOrderId=[" + param["gopayOutOrderId"] + "]tranIP=[" + param["tranIP"]
                + "]respCode=[" + param["respCode"] + "]gopayServerTime=[]VerficationCode=[" + MerchantKey + "]";
            logstr("回调:"+plain);
            string signature = md5(plain, Encoding.Default);
            OrderSn = param["merOrderNum"];
            return param["signValue"].Equals(signature) && param["respCode"].Equals("0000");
        }
        public override string WechatImgUrl(string OrderNo, decimal Amount, string backUrl, string BankCode, string subject, string body)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("version", "2.1");
            param.Add("charset", "2");
            param.Add("language", "1");
            param.Add("signType", "1");
            param.Add("tranCode", "SC01");
            param.Add("callType", "WX_WEB");
            param.Add("merchantID", MerchantNo);
            param.Add("merOrderNum", OrderNo);//商户订单号
            param.Add("tranAmt", Amount.ToString("f2"));//金额
            param.Add("feeAmt", "0");
            param.Add("currencyType", "156");//
            param.Add("backgroundMerUrl", backUrl);//后台通知地址
            param.Add("tranDateTime", Common.NowDate.ToString("yyyyMMddHHmmss"));
            param.Add("virCardNoIn", Param1);//国宝转入账户
            param.Add("tranIP", "127.0.0.1");//HttpContext.Current.Request.UserHostAddress
            param.Add("isRepeatSubmit", "1");
            param.Add("goodsName", subject);
            param.Add("goodsDetail", body);
            param.Add("merRemark", "");
            param.Add("goodsTag", "");
            param.Add("productId", "");
            param.Add("deviceInfo", "");
            param.Add("frontMerUrl", "");
            //获取国付宝时间
            url = "https://gateway.gopay.com.cn/time.do";
            param.Add("gopayServerTime", Request("", "", null));
            // 获取签名明文
            string[] jiami = { "version", "tranCode", "merchantID", "merOrderNum", "tranAmt", "tranDateTime", "backgroundMerUrl", "gopayServerTime", "tranIP", "callType", "goodsTag", "productId" };
            string signMsg = "";// getSignMsg(param);
            foreach (string key in jiami)
                signMsg += string.Format("{0}=[{1}]", key, param.ContainsKey(key) ? param[key] : "");
            signMsg += string.Format("{0}=[{1}]", "verficationCode", MerchantKey);
            logstr(signMsg);
            //encoding = Encoding.GetEncoding("GB2312");
            // 添加签名方法和签名
            param.Add("signValue", md5(signMsg, encoding));
            url = "https://gatewaymer.gopay.com.cn/Trans/APIClientAction.do";
            //url = backUrl.Replace("/back", "/go");
            string formData = "";
            foreach (KeyValuePair<string, string> item in param)
                formData += string.Format("{0}={1}&", item.Key, item.Value);
            method = "POST";
            string result = Request("", formData.TrimEnd('&'), null);
            throw new NotImplementedException();
        }
        public override string AlipayImgUrl(string OrderNo, decimal Amount, string backUrl, string BankCode, string subject, string body)
        {
            throw new NotImplementedException();
        }
    }
}