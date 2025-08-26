using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace OgrenciPortali.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class BufferRequestBodyAttribute : ActionFilterAttribute
    {
        private const string RequestBodyKey = "RequestBodyKey";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Items.Contains(RequestBodyKey))
            {
                var inputStream = filterContext.HttpContext.Request.InputStream;

                if (inputStream.CanSeek && inputStream.Length > 0)
                {
                    inputStream.Position = 0;
                    using (var reader = new StreamReader(inputStream))
                    {
                        var body = reader.ReadToEnd();
                        filterContext.HttpContext.Items[RequestBodyKey] = body;
                    }
                }
            }

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// Kaydedilen body'yi controller içinden okumak için yardımcı metod.
        /// </summary>
        public static string GetBody(HttpContextBase httpContext)
        {
            if (httpContext.Items.Contains(RequestBodyKey))
            {
                return httpContext.Items[RequestBodyKey] as string;
            }

            return null;
        }
    }
}