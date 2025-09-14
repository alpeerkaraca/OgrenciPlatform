using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;

namespace OgrenciPortali.Controllers
{
    public class ErrorController : Controller
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ErrorController));
        // GET: Error
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        public ActionResult ServerError()
        {
            Response.StatusCode = 500;
            return View();
        }

        public ActionResult AuthError()
        {
            Response.StatusCode = 401;
            return View();
        }

        public ActionResult AccessError()
        {
            Response.StatusCode = 403;
            return View();
        }
    }
}