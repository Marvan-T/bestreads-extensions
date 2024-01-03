using BestReads.Core.Responses;
using BestReads.Features.BookRecommendations.Dtos;
using BestReads.Features.BookRecommendations.Services.BookRecommendationService;
using Microsoft.AspNetCore.Mvc;

namespace BestReads.Features.BookRecommendations.Controllers;

[ApiController]
[Route("api/extensions/[controller]")]
public class BookRecommendationsController(
    IBookRecommendationService bookRecommendationService)
    : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ServiceResponse<List<BookRecommendationDto>>>> GenerateBookRecommendations(
        GetBookRecommendationsDto bookRecommendationsDto)
    {
        var serviceResponse = (await bookRecommendationService.GenerateRecommendations(bookRecommendationsDto))
            .Match(
                ServiceResponse<List<BookRecommendationDto>>.Success,
                ServiceResponse<List<BookRecommendationDto>>.Failure
            );

        return serviceResponse.Result.IsSuccess ? Ok(serviceResponse) : BadRequest(serviceResponse);
    }
}