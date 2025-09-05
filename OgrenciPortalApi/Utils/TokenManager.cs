using log4net;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using OgrenciPortalApi.Models;
using Shared.Enums;

namespace OgrenciPortalApi.Utils
{
    public class TokenManager
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TokenManager));
        private static readonly string Key = AppSettings.JwtMasterKey;


        public static string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var issuer = AppSettings.JwtIssuer;
            var audience = AppSettings.JwtAudience;
            var expMin = Convert.ToInt32(AppSettings.AccessTokenExpMins);
            var expTime = DateTime.UtcNow.AddMinutes(expMin);
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expTime,
                notBefore: DateTime.UtcNow,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GenerateRefreshToken()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rng.GetBytes(randomBytes);
                string base64 = Convert.ToBase64String(randomBytes);
                return base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
            }
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

                var principal =
                    tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                if (!(validatedToken is JwtSecurityToken jwtSecurityToken) ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase))
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

        public static IEnumerable<Claim> GetClaimsFromUser(Users user)
        {
            if (user == null)
                return new List<Claim>();
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim("user_email", user.Email.ToLower()),
                new Claim("full_name", (user.Name + " " + user.Surname)),
                new Claim(ClaimTypes.Role, Enum.GetName(typeof(Roles), user.Role)),
                new Claim("department", user.Departments?.Name ?? ""),
                new Claim("student_no", user.StudentNo ?? ""),
                new Claim("first_login", user.IsFirstLogin.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
            };
            return claims;
        }
    }
}