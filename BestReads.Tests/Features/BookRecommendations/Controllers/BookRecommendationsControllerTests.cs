using BestReads.Features.BookRecommendations.Controllers;
using BestReads.Features.BookRecommendations.Dtos;
using BestReads.Features.BookRecommendations.Errors;
using BestReads.Features.BookRecommendations.Services.BookRecommendationService;
using BestReads.Tests.Fakers;
using Microsoft.AspNetCore.Mvc;
using static BestReads.Tests.ControllerTestHelper;

namespace BestReads.Tests.Features.BookRecommendations.Controllers;

public class BookRecommendationsControllerTests
{
    private readonly BookRecommendationsController _controller;
    private readonly Mock<IBookRecommendationService> _mockService;

    public BookRecommendationsControllerTests()
    {
        _mockService = new Mock<IBookRecommendationService>();
        _controller = new BookRecommendationsController(_mockService.Object);
    }

    [Fact]
    public async Task GenerateBookRecommendations_WhenRequestIsValid_ReturnsListOfBookRecommendations()
    {
        // Arrange
        var bookRecommendationsDto = BookFakers.GetBookRecommendationDtoFaker().Generate();
        var bookList = BookFakers.BookRecommendationDtoFaker().Generate(2);

        var returningResult = CreateResult(bookList);
        var expectedServiceResponse = CreateServiceResponseFromResult(returningResult);
        _mockService.SetupMockServiceCall(service => service.GenerateRecommendations(bookRecommendationsDto),
            returningResult);

        // Act
        var result = await _controller.GenerateBookRecommendations(bookRecommendationsDto);

        // Assert
        CheckResponse(result, typeof(OkObjectResult), expectedServiceResponse);
    }

    [Fact]
    public async Task GenerateBookRecommendations_ForFailedServiceResponse_ReturnsBadRequest()
    {
        // Arrange
        var bookRecommendationsDto = BookFakers.GetBookRecommendationDtoFaker().Generate();
        var returningResult =
            CreateResult<List<BookRecommendationDto>>(null, false, GenerateRecommendationErrors.GoogleBooksIdNotFound);
        var expectedFailedServiceResponse = CreateServiceResponseFromResult(returningResult);

        _mockService.SetupMockServiceCall(service => service.GenerateRecommendations(bookRecommendationsDto),
            returningResult);

        // Act
        var result = await _controller.GenerateBookRecommendations(bookRecommendationsDto);

        // Assert
        CheckResponse(result, typeof(BadRequestObjectResult), expectedFailedServiceResponse);
    }
}