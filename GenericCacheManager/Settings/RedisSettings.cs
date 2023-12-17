namespace GenericCacheManager.Settings;

public class RedisSettings
{
    public bool Enabled { get; set; }
    public string ConnectionString { get; set; }
    public bool AllowAdmin { get; set; }
    public int Database { get; set; }
    public int CacheTimeOut { get; set; }
}