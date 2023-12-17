namespace GenericCacheManager.Cache;

public interface ICacheService
{
    void Add<T>(string key, T value);
    void Add<T>(string key, T value, TimeSpan? expireTime);
    void Add(string key, string value);
    void Add(string key, string value, TimeSpan? expireTime);
    Task<bool> AddAsync<T>(string key, T value);
    Task<bool> AddAsync<T>(string key, T value, TimeSpan? expireTime);
    Task<bool> AddAsync(string key, string value);
    Task<bool> AddAsync(string key, string value, TimeSpan? expireTime);
    T Get<T>(string key);
    Task<T> GetAsync<T>(string key);
    bool Delete(string key);
    Task<bool> DeleteAsync(string key);
    long FlushAllDatabases();
    Task<long> FlushAllDatabasesAsync();
}