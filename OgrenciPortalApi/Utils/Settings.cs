namespace OgrenciPortalApi.Utils
{
    public static class AppSettings
    {
        public static string JwtMasterKey { get; private set; }
        public static string JwtIssuer { get; private set; }
        public static string JwtAudience { get; private set; }
        public static string ApiBaseAddress { get; private set; }
        public static string AccessTokenExpMins { get; private set; }
        public static string RefreshTokenExpDays{ get; private set; }

        public static void Load()
        {
            JwtMasterKey = DotNetEnv.Env.GetString("JWT_MASTER_KEY");
            JwtIssuer = DotNetEnv.Env.GetString("JWT_ISSUER");
            JwtAudience = DotNetEnv.Env.GetString("JWT_AUDIENCE");
            ApiBaseAddress = DotNetEnv.Env.GetString("API_BASE_ADDRESS");
            AccessTokenExpMins = DotNetEnv.Env.GetString("ACCESS_TOKEN_EXPIRATION_MINUTES");
            RefreshTokenExpDays = DotNetEnv.Env.GetString("REFRESH_TOKEN_EXPIRATION_DAYS");
        }
    }
}