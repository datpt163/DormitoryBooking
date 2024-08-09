using StackExchange.Redis;
using System.Text.Json;

namespace RoomWebApi.Common.Caching
{
    public class RedisDbContext : ICachingDbContext
    {
        private IDatabase _cacheDb;

        public RedisDbContext()
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            _cacheDb = redis.GetDatabase();
        }

        public T? GetData<T>(string key) where T : class
        {
            var value = _cacheDb.StringGet(key).ToString();
            if (!string.IsNullOrEmpty(value))
            {
                var response = JsonSerializer.Deserialize<T>(value);
                return response;
            }
            return null;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expirTime = expirationTime.DateTime.Subtract(DateTime.Now);
            return _cacheDb.StringSet(key, JsonSerializer.Serialize(value), expirTime);

        }

        public object RemoveDate(string key)
        {
            var _exist = _cacheDb.KeyExists(key);

            if (_exist)
                return _cacheDb.KeyDelete(key);

            return false;
        }
    }
}
