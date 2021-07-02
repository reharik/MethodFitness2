using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MF.Web.Config
{
    public class CustomJsonResult : JsonResult
    {
        public CustomJsonResult(object input)
        {
            Data = input;
        }

        private const string _dateFormat = "yyyy-MM-ddTHH:mm:ss";

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            HttpResponseBase response = context.HttpContext.Response;

            if (!String.IsNullOrEmpty(ContentType))
            {
                response.ContentType = ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }
            if (Data != null)
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                // Using Json.NET serializer
                var isoConvert = new IsoDateTimeConverter();
                isoConvert.DateTimeFormat = _dateFormat;
                response.Write(JsonConvert.SerializeObject(Data, isoConvert));
            }
        }
    }
}