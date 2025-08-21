using StackExchange.Redis;

namespace OgrenciPortalApi.Utils
{
    public class CheckEmail
    {
        public static void Main(string[] args)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");
            IDatabase db = redis.GetDatabase();
            db.Ping();
        }
    }
}