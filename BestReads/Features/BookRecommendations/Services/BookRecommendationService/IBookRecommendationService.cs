using BestReads.Core.Responses;
using BestReads.Features.BookRecommendations.Dtos;

namespace BestReads.Features.BookRecommendations.Services.BookRecommendationService;

public interface IBookRecommendationService
{
    Task<ServiceResponse<List<BookRecommendationDto>>> GenerateRecommendations(
        GetBookRecommendationsDto bookRecommendationsDto);
}