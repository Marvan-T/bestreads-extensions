using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TestController(ILogger<TestController> logger) : ControllerBase
{
    [HttpGet]
    public void TestLogging()
    {
       var book = new { Author = "Hectar Gacia and Francesg Miralles", Title = "Ikigai"};
        logger.LogInformation("Hello from Serilog");
        logger.LogInformation("Process the book {@book}", book);
    }
}
