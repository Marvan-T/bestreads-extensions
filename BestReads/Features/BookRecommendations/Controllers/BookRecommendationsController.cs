using BestReads.Core.Responses;
using BestReads.Features.BookRecommendations.Dtos;
using BestReads.Features.BookRecommendations.Services.BookRecommendationService;
using Microsoft.AspNetCore.Mvc;

namespace BestReads.Features.BookRecommendations.Controllers;

[ApiController]
[Route("api/extensions/[controller]")]
public class BookRecommendationsController : ControllerBase
{
    private readonly IBookRecommendationService _bookRecommendationService;
    private readonly ILogger<BookRecommendationsController> _logger;


    public BookRecommendationsController(IBookRecommendationService bookRecommendationService,
        ILogger<BookRecommendationsController> logger)
    {
        _bookRecommendationService = bookRecommendationService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponse<List<BookRecommendationDto>>>> GenerateBookRecommendations(
        GetBookRecommendationsDto bookRecommendationsDto)
    {
        _logger.LogInformation($"Request received for GenerateBookRecommendations: {bookRecommendationsDto.Title}");

        var serviceResponse = (await _bookRecommendationService.GenerateRecommendations(bookRecommendationsDto))
            .Match(
                ServiceResponse<List<BookRecommendationDto>>.Success,
                ServiceResponse<List<BookRecommendationDto>>.Failure
            );

        return serviceResponse.Result.IsSuccess ? Ok(serviceResponse) : BadRequest(serviceResponse);
    }
}