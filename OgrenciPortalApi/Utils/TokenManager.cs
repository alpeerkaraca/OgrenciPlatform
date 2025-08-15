using log4net;
using Microsoft.IdentityModel.Tokens;
using OgrenciPortalApi.Models; // AppSettings için kendi projenizin using'ini ekleyin
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OgrenciPortalApi.Utils // Kendi projenizin ad alanını kullanın
{
    public class TokenManager
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TokenManager));
        private static readonly string Key = AppSettings.JwtMasterKey;


        public static string GenerateJwtToken(Users user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenHandler = new JwtSecurityTokenHandler();
            var now = DateTime.UtcNow;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.AuthenticationMethod, "Custom"),
                    new Claim("user_email", user.Email.ToLower()),
                    new Claim("full_name", (user.Name + " " + user.Surname)),
                    new Claim("user_role", user.Role.ToString()),
                    new Claim("department", user.Departments?.Name ?? ""),
                    new Claim("student_no", user.StudentNo ?? ""),
                    new Claim("first_login", user.IsFirstLogin.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
                }),
                Expires = now.AddMinutes(90),
                SigningCredentials = credentials,
                Issuer = AppSettings.JwtIssuer,
                Audience = AppSettings.JwtAudience
            };

            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                if (!(tokenHandler.ReadToken(token) is JwtSecurityToken))
                    return null;

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = AppSettings.JwtIssuer, 
                    ValidAudience = AppSettings.JwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key)),
                    ClockSkew = TimeSpan.Zero 
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                if (!(validatedToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    Logger.Warn("Token geçersiz bir imza algoritması ile geldi.");
                    return null;
                }

                return principal;
            }
            catch (SecurityTokenValidationException stvex)
            {
                Logger.Warn("Token doğrulaması başarısız oldu: " + stvex.Message);
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error("Token doğrulanırken beklenmedik bir hata oluştu: ", ex);
                return null;
            }
        }
    }
}
