using GenericCacheManager.Cache;
using Microsoft.AspNetCore.Mvc;

namespace GenericCacheManager.Controller;

[ApiController]
[Route("test")]
public class TestController : ControllerBase
{
    private readonly ICacheService _cacheService;

    public TestController(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    [HttpGet]
    [Route("create")]
    public async Task<IActionResult> GetUser()
    {
        await _cacheService.AddAsync("test2", new User
        {
            Name = "Fatih"
        }, TimeSpan.FromMilliseconds(8000));
        var result = _cacheService.Get<User>("test2");
        return Ok(result);
    }

    private class User
    {
        public string Name { get; set; }
    }
}