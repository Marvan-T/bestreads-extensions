using BestReads_Recommendations.Core.Responses;
using BestReads_Recommendations.Features.BookRecommendations.Dtos;
using BestReads_Recommendations.Features.BookRecommendations.Services.BookRecommendationService;
using Microsoft.AspNetCore.Mvc;

namespace BestReads_Recommendations.Features.BookRecommendations.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookRecommendationController : ControllerBase
{
    private readonly IBookRecommendationService _bookRecommendationService;

    public BookRecommendationController(IBookRecommendationService bookRecommendationService)
    {
        _bookRecommendationService = bookRecommendationService;
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponse<IList<BookRecommendationDto>>>> GetBookRecommendations(
        GetBookRecommendationsDto bookRecommendationsDto)

    {
        await _bookRecommendationService.GenerateRecommendations(bookRecommendationsDto);
        return Ok(null);
    }
}