using StackExchange.Redis;
using System;
using System.Configuration;

namespace OgrenciPortalApi.Utils
{
    public static class RedisService
    {

        private static readonly Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            try
            {
                var options = new ConfigurationOptions
                {
                    EndPoints = { { AppSettings.RedisHost, int.Parse(AppSettings.RedisPort) } },

                    Password = AppSettings.RedisPass,

                    Ssl = true,


                    ConnectTimeout = 5000,
                    SyncTimeout = 5000
                };

                return ConnectionMultiplexer.Connect(options);
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