namespace OgrenciPortali.Utils
{
    public static class AppSettings
    {
        public static string JwtMasterKey { get; private set; }
        public static string JwtIssuer { get; private set; }
        public static string JwtAudience { get; private set; }
        public static string ApiBaseAddress { get; private set; }
        public static string ClientId { get; private set; }
        public static string TenantId { get; private set; }
        public static string RedirectUri { get;private set; }
        public static string PostLogoutRedirectUri { get;private set; }

        public static void Load()
        {
            JwtMasterKey = DotNetEnv.Env.GetString("JWT_MASTER_KEY");
            JwtIssuer = DotNetEnv.Env.GetString("JWT_ISSUER");
            JwtAudience = DotNetEnv.Env.GetString("JWT_AUDIENCE");
            ApiBaseAddress = DotNetEnv.Env.GetString("API_BASE_ADDRESS");
            ClientId = DotNetEnv.Env.GetString("CLIENT_ID");
            TenantId = DotNetEnv.Env.GetString("TENANT_ID");
            RedirectUri = DotNetEnv.Env.GetString("REDIRECT_URI");
            PostLogoutRedirectUri = DotNetEnv.Env.GetString("POST_LOGOUT_REDIRECT_URI");
        }
    }
}