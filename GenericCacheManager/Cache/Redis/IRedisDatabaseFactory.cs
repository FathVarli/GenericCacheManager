namespace GenericCacheManager.Cache.Redis;

public interface IRedisDatabaseFactory
{
    RedisDatabase GetDatabase(int id = -1);
}