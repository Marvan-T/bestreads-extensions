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
        _logger.LogInformation($"Request: {bookRecommendationsDto.Title}");
        _logger.LogInformation("From Controller GoogleBooksId: {GoogleBooksId}", bookRecommendationsDto.GoogleBooksId);
        var response = await _bookRecommendationService.GenerateRecommendations(bookRecommendationsDto);
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }
}