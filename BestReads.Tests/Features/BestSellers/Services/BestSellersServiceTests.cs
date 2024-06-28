using BestReads.Features.BestSellers.Services.BestSellersService;
using BestReads.Infrastructure.ApiClients.NYTimes;
using BestReads.Infrastructure.ApiClients.NYTimes.DTOs;

namespace BestReads.Tests;

public class BestSellersServiceTests
{
    private readonly Mock<INYTimesApiClient> _mockNyTimesApiClient;
    private readonly BestSellersService _bestSellersService;

    public BestSellersServiceTests()
    {
        _mockNyTimesApiClient = new Mock<INYTimesApiClient>();
        _bestSellersService = new BestSellersService(_mockNyTimesApiClient.Object);
    }

    [Fact]
    public async Task GetBestSellersAsync_Success_ReturnsBestSellerListDto()
    {
        // Arrange
        var expectedBestSellerListDto = new BestSellerListDto();
        _mockNyTimesApiClient
            .Setup(x => x.GetCurrentBestSellersList())
            .ReturnsAsync(expectedBestSellerListDto);

        // Act
        var result = await _bestSellersService.GetBestSellersAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(expectedBestSellerListDto);
    }
}
