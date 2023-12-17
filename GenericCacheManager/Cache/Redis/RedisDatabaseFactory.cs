using GenericCacheManager.Settings;
using StackExchange.Redis;

namespace GenericCacheManager.Cache.Redis;

public class RedisDatabaseFactory : IRedisDatabaseFactory
{
    private readonly AppSettings _appSettings;
    private ConnectionMultiplexer _connectionMultiplexer;

    public RedisDatabaseFactory(AppSettings appSettings)
    {
        _appSettings = appSettings;
        TryConnect();
    }

    #region public RedisDatabase GetDatabase(int id = -1)

    public RedisDatabase GetDatabase(int id = -1)
    {
        var database = _connectionMultiplexer?.GetDatabase(id);

        if (database == null) TryConnect();

        return database == null ? null : new RedisDatabase(database);
    }

    #endregion

    #region private void TryConnect()

    private void TryConnect()
    {
        if (!_appSettings.RedisSettings.Enabled) return;

        var configurationOptions = ConfigurationOptions.Parse(_appSettings.RedisSettings.ConnectionString);
        configurationOptions.AllowAdmin = _appSettings.RedisSettings.AllowAdmin;
        _connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions);
    }

    #endregion
}