using log4net;
using Microsoft.IdentityModel.Tokens;
using OgrenciPortali.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;
using OgrenciPortali.Utils;

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
            try
            {
                if (httpContext.User is ClaimsPrincipal principal && principal.Identity.IsAuthenticated)
                {
                    return CheckRoleAuthorization(principal);
                }

                var token = ExtractTokenFromRequest(httpContext);
                if (string.IsNullOrEmpty(token))
                {
                    return false;
                }

                var validatedPrincipal = ValidateJwtToken(token);
                if (validatedPrincipal != null)
                {
                    httpContext.User = validatedPrincipal;
                    System.Threading.Thread.CurrentPrincipal = validatedPrincipal;

                    return CheckRoleAuthorization(validatedPrincipal);
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("Authorization error in CustomAuth: ", ex);
                return false;
            }
        }

        /// <summary>
        /// Extracts JWT token from Authorization header or session/cookie
        /// </summary>
        private string ExtractTokenFromRequest(HttpContextBase httpContext)
        {
            // Try Authorization header first
            var authHeader = httpContext.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return authHeader.Substring("Bearer ".Length).Trim();
            }

            // Try session
            if (httpContext.Session["bearerToken"] != null)
            {
                return httpContext.Session["BearerToken"].ToString();
            }

            // Try TempData if available in controller context
            var controller = httpContext.Handler as Controller;
            if (controller?.TempData["BearerToken"] != null)
            {
                return controller.TempData["BearerToken"].ToString();
            }

            return null;
        }

        /// <summary>
        /// Validates JWT token manually using the same logic as TokenManager
        /// </summary>
        private ClaimsPrincipal ValidateJwtToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                if (!(tokenHandler.ReadToken(token) is JwtSecurityToken jwtToken))
                    return null;

                var key = AppSettings.JwtMasterKey;
                var symmetricKey = Encoding.UTF8.GetBytes(key);

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = AppSettings.JwtIssuer,
                    ValidAudience = AppSettings.JwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch (SecurityTokenExpiredException ex)
            {
                Logger.Error("Token expired in CustomAuth: ", ex);
                return null;
            }
            catch (SecurityTokenException ex)
            {
                Logger.Error("Token validation failed in CustomAuth: ", ex);
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error("Unexpected error during token validation in CustomAuth: ", ex);
                return null;
            }
        }

        /// <summary>
        /// Checks if the authenticated user has the required roles
        /// </summary>
        private bool CheckRoleAuthorization(ClaimsPrincipal principal)
        {
            if (_allowedRoles == null || !_allowedRoles.Any())
                return true; 

            var userRoles = principal.FindAll("user_role").Select(c => c.Value).ToHashSet();

            if (!userRoles.Any())
                return false; 

            foreach (var allowedRole in _allowedRoles)
            {
                if (userRoles.Contains(allowedRole.ToString()))
                {
                    return true;
                }

                if (userRoles.Contains(((int)allowedRole).ToString()))
                {
                    return true;
                }
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
        {
            try
            {
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

                ClearAuthenticationData(filterContext.HttpContext);
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

        /// <summary>
        /// Clears authentication data when unauthorized
        /// </summary>
        private void ClearAuthenticationData(HttpContextBase httpContext)
        {
            try
            {
                // Clear session data
                if (httpContext.Session != null)
                {
                    httpContext.Session.Remove("bearerToken");
                }

                // Clear TempData if available
                var controller = httpContext.Handler as Controller;
                if (controller?.TempData != null)
                {
                    controller.TempData.Remove("bearerToken");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error clearing authentication data: ", ex);
            }
        }
    }
}