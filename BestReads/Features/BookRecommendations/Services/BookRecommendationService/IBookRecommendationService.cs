using BestReads.Core.Utilities;
using BestReads.Features.BookRecommendations.Dtos;

namespace BestReads.Features.BookRecommendations.Services.BookRecommendationService;

public interface IBookRecommendationService
{
    Task<Result<List<BookRecommendationDto>>> GenerateRecommendations(
        GetBookRecommendationsDto bookRecommendationsDto);
}