using System;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using OgrenciPortalApi.Models;
using StackExchange.Redis;

namespace OgrenciPortalApi.Utils
{
    public class CheckUserData
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CheckUserData));
        private static readonly IDatabase redisDb = RedisService.GetDatabase();
        public static async Task CacheUserAddressesAsync()
        {
            try
            {
                using (var sql = new OgrenciPortalApiDB())
                {
                    var users = sql.Users.Select(u => new { u.UserId, u.Email }).ToList();
                    if (!users.Any())
                        return;

                    var tasks = users.Select(user =>
                    {
                        string redisKey = $"user:email:{user.Email}";
                        return redisDb.StringSetAsync(redisKey, user.UserId.ToString());
                    });

                    await Task.WhenAll(tasks);
                    Logger.Info($"{users.Count} kullanıcı e-postası başarıyla Redis'e önbelleklendi.");
                }
            }
            catch (Exception e)
            {
                Logger.Error("Kullanıcı verileri Redis'e önbelleklenirken bir hata oluştu: " + e.Message, e);
            }
        }

        private static async Task<bool> CheckEmailAddressOnRedisAsync(string email)
        {
            return await redisDb.KeyExistsAsync($"user:email:{email}");
        }

        private static async Task<bool> CheckEmailAddressOnSqlAsync(string email)
        {
            using (var sql = new OgrenciPortalApiDB())
            {
                return await Task.FromResult(sql.Users.Any(u => u.Email == email));
            }
        }

        public static async Task<bool> CheckEmailAddressAsync(string email)
        {
            if (await CheckEmailAddressOnRedisAsync(email))
            {
                return true;
            }

            return await CheckEmailAddressOnSqlAsync(email);
        }
    }
}