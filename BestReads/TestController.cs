using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TestController(ILogger<TestController> logger) : ControllerBase
{
    [HttpGet]
    public void TestLogging()
    {
        logger.LogInformation("Hello from Serilog");
    }
}
