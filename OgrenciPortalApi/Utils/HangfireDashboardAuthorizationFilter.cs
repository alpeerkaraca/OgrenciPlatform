
using Hangfire.Dashboard;
using Microsoft.Owin;
using System.Security.Claims;
using Shared.Enums;

public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var owinContext = new OwinContext(context.GetOwinEnvironment());

        if (owinContext.Authentication.User == null || !owinContext.Authentication.User.Identity.IsAuthenticated)
        {
            return false;
        }

        bool isAdmin = owinContext.Authentication.User.HasClaim(ClaimTypes.Role, nameof(Roles.Admin));

        return isAdmin;
    }
}