using BestReads_Recommendations.Core.Responses;
using BestReads_Recommendations.Features.BookRecommendations.Dtos;
using BestReads_Recommendations.Features.BookRecommendations.Services.BookRecommendationService;
using Microsoft.AspNetCore.Http.HttpResults;
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

    [HttpGet]
    public async void GetBookRecommendations(
        GetBookRecommendationsDto bookRecommendationsDto)
    {
        await _bookRecommendationService.GenerateRecommendations(bookRecommendationsDto);
    }
}