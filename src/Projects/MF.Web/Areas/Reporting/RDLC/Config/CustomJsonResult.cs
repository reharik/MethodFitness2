//using System;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Converters;
//using Microsoft.AspNetCore.Mvc;

//namespace MF.Web.Config
//{
//    public class JsonResult : JsonResult
//    {
//        public JsonResult(object input)
//        {
//            Data = input;
//        }

//        private const string _dateFormat = "yyyy-MM-ddTHH:mm:ss";

//        public override void ExecuteResultAsync(ControllerContext context)
//        {
//            if (context == null)
//            {
//                throw new ArgumentNullException("context");
//            }

//            HttpResponseBase response = context.HttpContext.Response;

//            if (!String.IsNullOrEmpty(ContentType))
//            {
//                response.ContentType = ContentType;
//            }
//            else
//            {
//                response.ContentType = "application/json";
//            }
//            if (ContentEncoding != null)
//            {
//                response.ContentEncoding = ContentEncoding;
//            }
//            if (Data != null)
//            {
//                JsonRequestBehavior = JsonRequestBehavior.AllowGet;
//                // Using Json.NET serializer
//                var isoConvert = new IsoDateTimeConverter();
//                isoConvert.DateTimeFormat = _dateFormat;
//                response.Write(JsonConvert.SerializeObject(Data, isoConvert));
//            }
//        }
//    }
//}