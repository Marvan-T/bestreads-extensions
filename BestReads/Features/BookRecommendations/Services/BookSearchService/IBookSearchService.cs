using BestReads.Core;
using BestReads.Core.Utilities;
using BestReads.Features.BookRecommendations.Dtos;

namespace BestReads.Features.BookRecommendations.Services.BookSearchService;

public interface IBookSearchService
{
    Task<Result<List<BookRecommendationDto>>> GetNearestNeighbors(Book book);
}