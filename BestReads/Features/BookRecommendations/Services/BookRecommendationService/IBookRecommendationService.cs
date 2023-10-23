using BestReads.Core;
using BestReads.Core.Responses;
using BestReads.Features.BookRecommendations.Dtos;

namespace BestReads.Features.BookRecommendations.Services.BookRecommendationService;

public interface IBookRecommendationService
{
    Task<ServiceResponse<IList<BookRecommendationDto>>> GenerateRecommendations(GetBookRecommendationsDto bookRecommendationsDto);
}