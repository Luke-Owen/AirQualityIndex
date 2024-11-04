using System.Text.Json;
using StackExchange.Redis;
using AirQualityIndex.Interfaces;

namespace AirQualityIndex.Services;

public class RedisService(IConnectionMultiplexer redis) : IRedisService
{
    private readonly IConnectionMultiplexer _redis = redis ?? throw new ArgumentNullException(nameof(redis));

    private IDatabase GetDatabase() => _redis.GetDatabase();

    // Basic String operations
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var db = GetDatabase();

        var serializedValue = JsonSerializer.Serialize(value);
        
        await db.StringSetAsync(key, serializedValue, expiry);
    }

    public async Task<T> GetAsync<T>(string key)
    {
        var db = GetDatabase();
        var serializedValue = await db.StringGetAsync(key);
        return string.IsNullOrEmpty(serializedValue) 
            ? default 
            : JsonSerializer.Deserialize<T>(serializedValue);
    }

    // Set operations
    public async Task AddToSetAsync(string key, string value)
    {
        var db = GetDatabase();
        await db.SetAddAsync(key, value);
    }

    public async Task<IEnumerable<string>> GetSetMembersAsync(string key)
    {
        var db = GetDatabase();
        var members = await db.SetMembersAsync(key);
        return Array.ConvertAll(members, v => v.ToString());
    }

    // Hash operations
    public async Task SetHashFieldAsync(string hashKey, string field, string value)
    {
        var db = GetDatabase();
        await db.HashSetAsync(hashKey, field, value);
    }

    public async Task<string> GetHashFieldAsync(string hashKey, string field)
    {
        var db = GetDatabase();
        return await db.HashGetAsync(hashKey, field);
    }

    public async Task<Dictionary<string, string>> GetAllHashFieldsAsync(string hashKey)
    {
        var db = GetDatabase();
        var hashEntries = await db.HashGetAllAsync(hashKey);
        var result = new Dictionary<string, string>();
        foreach (var entry in hashEntries)
        {
            result[entry.Name] = entry.Value;
        }
        return result;
    }

    // List operations
    public async Task PushToListAsync(string key, string value, bool prepend = false)
    {
        var db = GetDatabase();
        if (prepend)
        {
            await db.ListLeftPushAsync(key, value);
        }
        else
        {
            await db.ListRightPushAsync(key, value);
        }
    }

    public async Task<IEnumerable<string>> GetListAsync(string key)
    {
        var db = GetDatabase();
        var length = await db.ListLengthAsync(key);
        var values = await db.ListRangeAsync(key, 0, length - 1);
        return Array.ConvertAll(values, v => v.ToString());
    }

    // Expiration management
    public async Task SetKeyExpiryAsync(string key, TimeSpan expiry)
    {
        var db = GetDatabase();
        await db.KeyExpireAsync(key, expiry);
    }

    public async Task<bool> KeyExistsAsync(string key)
    {
        var db = GetDatabase();
        return await db.KeyExistsAsync(key);
    }

    public async Task DeleteKeyAsync(string key)
    {
        var db = GetDatabase();
        await db.KeyDeleteAsync(key);
    }
}
