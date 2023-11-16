using BestReads.Core;
using BestReads.Features.BookRecommendations.Dtos;
using BestReads.Infrastructure.AzureSearchClient;

namespace BestReads.Features.BookRecommendations.Services.BookSearchService;

public class BookSearchService : IBookSearchService
{
    private readonly IAzureSearchClient _azureSearchClient;

    public BookSearchService(IAzureSearchClient azureSearchClient)
    {
        _azureSearchClient = azureSearchClient;
    }

    public async Task<List<BookRecommendationDto>> GetNearestNeighbors(Book book)
    {
        var filter = $"GoogleBooksId ne '{book.GoogleBooksId}' and Title ne '{book.Title}'";
        return await _azureSearchClient.SingleVectorSearch<BookRecommendationDto>(book.Embeddings,
            "Embeddings", filter,
            new List<string>
            {
                "doc_id, GoogleBooksId, Title, Authors, Categories, Description, Publisher, PublishedDate, Thumbnail, IndustryIdentifiers"
            });
    }
}