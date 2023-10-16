using BestReads_Recommendations.Core;
using BestReads_Recommendations.Core.Responses;
using BestReads_Recommendations.Features.BookRecommendations.Dtos;

namespace BestReads_Recommendations.Features.BookRecommendations.Services.BookRecommendationService;

public interface IBookRecommendationService
{
    Task<ServiceResponse<IList<BookRecommendationDto>>> GenerateRecommendations(GetBookRecommendationsDto bookRecommendationsDto);
}