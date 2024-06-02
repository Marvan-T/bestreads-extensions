using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BestReads.Tests;

public class GlobalExceptionHandlerIntegrationTests
{
    private readonly HttpClient _client;
    private readonly TestServer _server;

    public GlobalExceptionHandlerIntegrationTests()
    {
        var webHostHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddControllers();
                services.AddExceptionHandler<GlobalExceptionHandler>();
                services.AddProblemDetails();
            })
            .Configure(app =>
            {
                app.UseExceptionHandler();
                app.Run(context => { throw new Exception("Test exception"); });
            });

        _server = new TestServer(webHostHostBuilder);
        _client = _server.CreateClient();
    }


    [Fact]
    public async Task WhenExceptionOccurs_ThenReturnsInternalServerError()
    {
        var response = await _client.GetAsync("/");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var content = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

        responseObject.Should().NotBeNull();

        var resultJObject = responseObject["result"] as JObject;
        resultJObject.Should().NotBeNull();

        resultJObject["isSuccess"].Value<bool>().Should().BeFalse();

        var errorJObject = resultJObject["error"] as JObject;
        errorJObject.Should().NotBeNull();

        errorJObject["code"].Value<string>().Should().Be(GlobalExceptionHandler.ServerError.Code);
        errorJObject["description"].Value<string>().Should().Be(GlobalExceptionHandler.ServerError.Description);
    }
}