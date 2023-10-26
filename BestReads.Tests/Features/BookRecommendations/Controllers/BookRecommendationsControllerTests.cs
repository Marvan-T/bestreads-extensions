using BestReads.Features.BookRecommendations.Controllers;
using BestReads.Features.BookRecommendations.Dtos;
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
        var bookRecommendationsDto = new GetBookRecommendationsDto(); 
        var bookList = BookFakers.BookRecommendationDtoFaker().Generate(2);

        var expectedServiceResponse = CreateServiceResponse(bookList);
        _mockService.SetupMockServiceCall(service => service.GenerateRecommendations(bookRecommendationsDto),
            expectedServiceResponse);

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
        var expectedFailedServiceResponse =
            CreateServiceResponse<List<BookRecommendationDto>>(null, false);
        _mockService.SetupMockServiceCall(service => service.GenerateRecommendations(bookRecommendationsDto),
            expectedFailedServiceResponse);

        // Act
        var result = await _controller.GenerateBookRecommendations(bookRecommendationsDto);

        // Assert
        CheckResponse(result, typeof(BadRequestObjectResult), expectedFailedServiceResponse);
    }
}