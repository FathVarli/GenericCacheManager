using GenericCacheManager.Cache;
using GenericCacheManager.Cache.Redis;
using GenericCacheManager.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AppSettings>(builder.Configuration);
var appSettings = builder.Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
builder.Services.AddSingleton(appSettings);
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = appSettings.RedisSettings.ConnectionString;
});

builder.Services.AddSingleton<IRedisDatabaseFactory, RedisDatabaseFactory>();
builder.Services.AddSingleton<ICacheService, RedisService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();

app.Run();