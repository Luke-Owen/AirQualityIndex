namespace AirQualityIndex.Interfaces;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IRedisService
{
    // Basic String operations
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<T> GetAsync<T>(string key);

    // Set operations
    Task AddToSetAsync(string key, string value);
    Task<IEnumerable<string>> GetSetMembersAsync(string key);

    // Hash operations
    Task SetHashFieldAsync(string hashKey, string field, string value);
    Task<string> GetHashFieldAsync(string hashKey, string field);
    Task<Dictionary<string, string>> GetAllHashFieldsAsync(string hashKey);

    // List operations
    Task PushToListAsync(string key, string value, bool prepend = false);
    Task<IEnumerable<string>> GetListAsync(string key);

    // Expiration management
    Task SetKeyExpiryAsync(string key, TimeSpan expiry);
    Task<bool> KeyExistsAsync(string key);
    Task DeleteKeyAsync(string key);
}
