using log4net;
using Shared.Enums;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OgrenciPortali.Attributes
{
    public class CustomAuthAttribute : AuthorizeAttribute
    {
        private readonly Roles[] _allowedRoles;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CustomAuthAttribute));

        public CustomAuthAttribute(params Roles[] roles)
        {
            this._allowedRoles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.User?.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            if (_allowedRoles == null || !_allowedRoles.Any())
            {
                return true;
            }

            return _allowedRoles.Any(role => httpContext.User.IsInRole(role.ToString()));
        }

        protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
        {
            try
            {
                ClearAuthenticationData(filterContext.HttpContext);

                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult
                    {
                        Data = new
                        {
                            success = false,
                            message = "Oturum süreniz dolmuş. Lütfen tekrar giriş yapın.",
                            redirectUrl = "/User/Login"
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                    filterContext.HttpContext.Response.StatusCode = 401;
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new System.Web.Routing.RouteValueDictionary
                        {
                            { "controller", "User" },
                            { "action", "Login" },
                            { "returnUrl", filterContext.HttpContext.Request.Url?.PathAndQuery }
                        });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error handling unauthorized request: ", ex);

                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                        { "controller", "User" },
                        { "action", "Login" }
                    });
            }
        }

        private void ClearAuthenticationData(HttpContextBase httpContext)
        {
            try
            {
                var cookie = new HttpCookie("AuthToken", string.Empty)
                {
                    Expires = DateTime.Now.AddDays(-1),
                    HttpOnly = true,
                    Path = "/"
                };
                httpContext.Response.Cookies.Add(cookie);
            }
            catch (Exception ex)
            {
                Logger.Error("Error clearing authentication data: ", ex);
            }
        }
    }
}