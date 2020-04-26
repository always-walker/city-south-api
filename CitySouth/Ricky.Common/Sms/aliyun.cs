using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public class aliyun : SMS
    {
        public override bool Send(string phone, string content)
        {
            url = "http://sms.market.alicloudapi.com/singleSendSms";

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", "APPCODE " + appkey);
            JObject result = JObject.Parse(Request("{\"no\":\"123456\"}", "", headers));
            return (bool)result["success"];
        }
        public override bool Send(string phone, string content, string template)
        {
            throw new NotImplementedException();
        }
    }
}
