using BestReads.Core;
using BestReads.Features.BookRecommendations.Dtos;

namespace BestReads.Features.BookRecommendations.Services.BookSearchService;

public interface IBookSearchService
{
    Task<List<BookRecommendationDto>> GetNearestNeighbors(Book book);
}