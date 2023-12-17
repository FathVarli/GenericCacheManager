using System.Text;
using System.Text.Json;
using GenericCacheManager.Settings;
using StackExchange.Redis;

namespace GenericCacheManager.Cache.Redis;

public class RedisService : ICacheService
{
    private readonly AppSettings _appSettings;
    private readonly IDatabase _database;
    private readonly bool _enabled;
    private bool _available;

    public RedisService(AppSettings appSettings, IRedisDatabaseFactory redisDatabaseFactory)
    {
        _appSettings = appSettings;
        var redisDatabase = redisDatabaseFactory.GetDatabase(appSettings.RedisSettings.Database);
        _database = redisDatabase?.Database;
        _enabled = _appSettings.RedisSettings.Enabled;
        _available = _database != null && _appSettings.RedisSettings.Enabled;
    }

    public void Add<T>(string key, T value)
    {
        TryExecute(database => database.StringSet(GetKey(key), ToJson(value)));
    }

    public void Add<T>(string key, T value, TimeSpan? expireTime)
    {
        TryExecute(database => database.StringSet(GetKey(key), ToJson(value), expireTime));
    }

    public void Add(string key, string value)
    {
        TryExecute(database => database.StringSet(GetKey(key), value));
    }

    public void Add(string key, string value, TimeSpan? expireTime)
    {
        TryExecute(database => database.StringSet(GetKey(key), value, expireTime));
    }

    public async Task<bool> AddAsync<T>(string key, T value)
    {
        return await TryExecuteAsync(database => database.StringSetAsync(GetKey(key), ToJson(value)))
            .ConfigureAwait(false);
    }
    
    public async Task<bool> AddAsync<T>(string key, T value, TimeSpan? expireTime)
    {
        return await TryExecuteAsync(database => database.StringSetAsync(GetKey(key), ToJson(value), expireTime))
            .ConfigureAwait(false);
    }

    public async Task<bool> AddAsync(string key, string value)
    {
        return await TryExecuteAsync(database => database.StringSetAsync(GetKey(key), value))
            .ConfigureAwait(false);
    }

    public async Task<bool> AddAsync(string key, string value, TimeSpan? expireTime)
    {
        return await TryExecuteAsync(database => database.StringSetAsync(GetKey(key), value, expireTime))
            .ConfigureAwait(false);
    }

    public T Get<T>(string key)
    {
        var redisKey = GetKey(key);
        var redisValue = TryExecute(database => database.StringGet(redisKey));

        if (redisValue.IsNullOrEmpty) return default;

        var jsonValue = Encoding.UTF8.GetString(redisValue);
        var value = FromJson<T>(jsonValue);
        return value;
    }

    public async Task<T> GetAsync<T>(string key)
    {
        var redisKey = GetKey(key);
        var redisValue = await TryExecuteAsync(database => database.StringGetAsync(redisKey)).ConfigureAwait(false);

        if (redisValue.IsNullOrEmpty) return default;


        var jsonValue = Encoding.UTF8.GetString(redisValue);
        var value = FromJson<T>(jsonValue);
        return value;
    }

    public bool Delete(string key)
    {
        return TryExecute(database => database.KeyDelete(GetKey(key)));
    }

    public async Task<bool> DeleteAsync(string key)
    {
        return await TryExecuteAsync(database => database.KeyDeleteAsync(GetKey(key))).ConfigureAwait(false);
    }

    public long FlushAllDatabases()
    {
        var deletedCount = 0;
        var connectionMultiplexer = _database.Multiplexer;
        var endPoints = connectionMultiplexer.GetEndPoints();
        foreach (var endPoint in endPoints)
        {
            var server = connectionMultiplexer.GetServer(endPoint);
            server.FlushAllDatabases();
            deletedCount++;
        }

        return deletedCount;
    }

    public async Task<long> FlushAllDatabasesAsync()
    {
        var deletedCount = 0;
        var connectionMultiplexer = _database.Multiplexer;
        var endPoints = connectionMultiplexer.GetEndPoints();
        foreach (var endPoint in endPoints)
        {
            var server = connectionMultiplexer.GetServer(endPoint);
            await server.FlushAllDatabasesAsync();
            deletedCount++;
        }

        return deletedCount;
    }

    private FType TryExecute<FType>(Func<IDatabase, FType> database)
    {
        if (!_enabled) return default;

        try
        {
            var result = database(_database);
            if (_available) return result;

            _available = true;
            return result;
        }
        catch (Exception ex)
        {
            if (!_available) return default;

            _available = false;
        }

        return default;
    }

    private async Task<FType> TryExecuteAsync<FType>(Func<IDatabase, Task<FType>> database)
    {
        if (!_enabled) return default;

        try
        {
            var result = await database(_database).ConfigureAwait(false);
            if (_available) return result;

            _available = true;

            return result;
        }
        catch (Exception ex)
        {
            if (!_available) return default;

            _available = false;
        }

        return default;
    }

    private static string GetKey(string key)
    {
        return key.ToLowerInvariant();
    }

    private static string ToJson(object value)
    {
        return value != null ? JsonSerializer.Serialize(value) : default;
    }

    private static T? FromJson<T>(string value)
    {
        if (string.IsNullOrEmpty(value)) return default;

        value = value.Trim();

        if (typeof(T) == typeof(string)) return (T)(object)value;


        return JsonSerializer.Deserialize<T>(value);
    }
}