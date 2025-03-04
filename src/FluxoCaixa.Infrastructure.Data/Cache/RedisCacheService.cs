using FluxoCaixa.Domain.Core.Interfaces;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace FluxoCaixa.Infrastructure.Data.Cache
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private readonly string _instanceName;
        private readonly TimeSpan _defaultExpiration;

        public RedisCacheService(IConnectionMultiplexer redis, IOptions<RedisSettings> settings)
        {
            _redis = redis;
            _database = redis.GetDatabase();
            _instanceName = settings.Value.InstanceName;
            _defaultExpiration = TimeSpan.FromMinutes(settings.Value.DefaultExpirationInMinutes);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var fullKey = $"{_instanceName}{key}";
            var value = await _database.StringGetAsync(fullKey);

            if (value.IsNullOrEmpty)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(value!);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var fullKey = $"{_instanceName}{key}";
            var serializedValue = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(fullKey, serializedValue, expiry ?? _defaultExpiration);
        }

        public async Task RemoveAsync(string key)
        {
            var fullKey = $"{_instanceName}{key}";
            await _database.KeyDeleteAsync(fullKey);
        }
    }

    public class RedisSettings
    {
        public string InstanceName { get; set; } = "FluxoCaixa:";
        public int DefaultExpirationInMinutes { get; set; } = 60;
    }
} 