using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CitySouth
{
    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            string Name = headers.ContentDisposition.FileName.Replace("\"", string.Empty);

            if (Name.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase) ||
                 Name.EndsWith(".png", StringComparison.CurrentCultureIgnoreCase) ||
                 Name.EndsWith(".gif", StringComparison.CurrentCultureIgnoreCase) ||
                Name.EndsWith(".xls", StringComparison.CurrentCultureIgnoreCase) ||
                Name.EndsWith(".xlsx", StringComparison.CurrentCultureIgnoreCase)
                )
            {
                return headers.ContentDisposition.GetHashCode() + "_" + Name;
            }
            else
            {
                throw new InvalidOperationException("上传格式错误");
            }
        }
    }
}
