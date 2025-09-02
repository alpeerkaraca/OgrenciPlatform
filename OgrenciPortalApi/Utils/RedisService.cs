using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using StackExchange.Redis;

namespace OgrenciPortalApi.Utils
{
    public static class RedisService
    {
        private static string GetRedisConnectionString()
        {
            return $"{AppSettings.RedisHost},user={AppSettings.RedisUser},password={AppSettings.RedisPass}";
        }

        private static readonly Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            try
            {
                return ConnectionMultiplexer.Connect(GetRedisConnectionString());

            }
            catch (Exception e)
            {
                Console.WriteLine($"Redis bağlantı hatası: {e.Message}");
                throw;
            }
        });

        private static ConnectionMultiplexer Connection => lazyConnection.Value;

        public static IDatabase GetDatabase(int db = -1)
        {
            return Connection.GetDatabase(db);
        }

    }
}