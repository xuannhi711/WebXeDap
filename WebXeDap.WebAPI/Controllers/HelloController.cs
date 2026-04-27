using Microsoft.AspNetCore.Mvc;

namespace WebXeDap.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class HelloController : ControllerBase
{
    private readonly ILogger<HelloController> _logger;

    public HelloController(ILogger<HelloController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public string Get()
    {
        return "Hello, World!";
    }
}
