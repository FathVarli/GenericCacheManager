using StackExchange.Redis;

namespace GenericCacheManager.Cache.Redis;

public class RedisDatabase : IRedisDatabase
{
    public RedisDatabase(IDatabase database)
    {
        Database = database;
    }

    public IDatabase Database { get; set; }
}