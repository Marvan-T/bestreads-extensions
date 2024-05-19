using System.Net;
using System.Web;
using BestReads.Infrastructure.ApiClients.NYTimes.Handlers;
using Bogus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq.Protected;

namespace BestReads.Tests.Handlers;

public class NyTimesAuthenticationDelegatingHandlerTests
{
    private const string ApiKeyHeaderName = "api-key";
    private readonly string ApiKey = new Faker().Random.AlphaNumeric(10);

    [Fact]
    public async Task SendAsync_ShouldAddApiKeyHeader()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>
        {
            { "NYTimes:ApiKey", ApiKey }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var mockLogger = new Mock<ILogger<NyTimesAuthenticationDelegatingHandler>>();

        var handler = new NyTimesAuthenticationDelegatingHandler(configuration, mockLogger.Object)
        {
            InnerHandler = new Mock<HttpMessageHandler>().Object
        };

        var invoker = new HttpMessageInvoker(handler);
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.nytimes.com/test");

        var mockInnerHandler = Mock.Get(handler.InnerHandler);
        mockInnerHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        await invoker.SendAsync(request, CancellationToken.None);

        // Assert
        var queryParams = HttpUtility.ParseQueryString(request.RequestUri.Query);
        queryParams[ApiKeyHeaderName].Should().Be(ApiKey);
    }
}