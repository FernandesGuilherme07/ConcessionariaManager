using ConcessionariaManager.Core.Interfaces.Services;
using StackExchange.Redis;
using System.Text.Json;

namespace ConcessionariaManager.Web.ExternalServices
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly IDatabase _database;

        public RedisCacheService(IConnectionMultiplexer connection)
        {
            _connection = connection;
            _database = _connection.GetDatabase();
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _database.StringGetAsync(key);
            return value.HasValue ? JsonSerializer.Deserialize<T>(value!) : default;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var json = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, json, expiry);
        }

        public async Task RemoveAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }
    }

}
