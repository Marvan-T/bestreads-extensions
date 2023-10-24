using BestReads.Core.Responses;
using BestReads.Features.BookRecommendations.Dtos;
using BestReads.Features.BookRecommendations.Services.BookRecommendationService;
using Microsoft.AspNetCore.Mvc;

namespace BestReads.Features.BookRecommendations.Controllers;

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
    public async Task<ActionResult<ServiceResponse<IList<BookRecommendationDto>>>> GenerateBookRecommendations(
        GetBookRecommendationsDto bookRecommendationsDto)

    {
        var response = await _bookRecommendationService.GenerateRecommendations(bookRecommendationsDto);
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }
}