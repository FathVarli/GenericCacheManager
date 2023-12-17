using StackExchange.Redis;

namespace GenericCacheManager.Cache.Redis;

public interface IRedisDatabase
{
    IDatabase Database { get; set; }
}