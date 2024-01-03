using BestReads.Core;
using BestReads.Core.Utilities;
using BestReads.Features.BookRecommendations.Dtos;
using BestReads.Features.BookRecommendations.Errors;
using BestReads.Infrastructure.AzureSearchClient;

namespace BestReads.Features.BookRecommendations.Services.BookSearchService;

public class BookSearchService(IAzureSearchClient azureSearchClient) : IBookSearchService
{
    public async Task<Result<List<BookRecommendationDto>>> GetNearestNeighbors(Book book)
    {
        var filter = $"GoogleBooksId ne '{book.GoogleBooksId}' and Title ne '{book.Title}'";
        var recommendations = await azureSearchClient.SingleVectorSearch<BookRecommendationDto>(book.Embeddings,
            "Embeddings", filter,
            new List<string>
            {
                "doc_id, GoogleBooksId, Title, Authors, Categories, Description, Publisher, PublishedDate, Thumbnail, IndustryIdentifiers"
            });

        return recommendations.Count == 0
            ? Result<List<BookRecommendationDto>>.Failure(GenerateRecommendationErrors.RecommendationsNotFound)
            : Result<List<BookRecommendationDto>>.Success(recommendations);
    }
}