using AutoMapper;
using BestReads.Core;
using BestReads.Core.Utilities;
using BestReads.Features.BookRecommendations.Dtos;
using BestReads.Features.BookRecommendations.Errors;
using BestReads.Features.BookRecommendations.Repository;
using BestReads.Features.BookRecommendations.Services.BookEmbeddingService;
using BestReads.Features.BookRecommendations.Services.BookSearchService;

namespace BestReads.Features.BookRecommendations.Services.BookRecommendationService;

public class BookRecommendationService(
    IBookRepository bookRepository,
    IBookEmbeddingService bookEmbeddingService,
    IMapper mapper,
    ILogger<BookRecommendationService> logger,
    IBookSearchService bookSearchService,
    IConfiguration configuration)
    : IBookRecommendationService
{
    public async Task<Result<List<BookRecommendationDto>>> GenerateRecommendations(
        GetBookRecommendationsDto bookRecommendationsDto)
    {
        try
        {
            var bookResult = await GetOrStoreBookWithEmbeddingsAsync(bookRecommendationsDto);
            if (!bookResult.IsSuccess) return Result<List<BookRecommendationDto>>.Failure(bookResult.Error);

            return await GetRecommendationsAsync(bookResult.Data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while generating recommendations for {GoogleBooksId}",
                bookRecommendationsDto.GoogleBooksId);
            return Result<List<BookRecommendationDto>>.Failure(new Error("UnexpectedError", ex.Message));
        }
    }

    private async Task<Result<Book>> GetOrStoreBookWithEmbeddingsAsync(GetBookRecommendationsDto bookRecommendationsDto)
    {
        if (string.IsNullOrEmpty(bookRecommendationsDto.GoogleBooksId))
            return Result<Book>.Failure(GenerateRecommendationErrors.GoogleBooksIdNotFound);

        var book = await bookRepository.GetByGoogleBooksIdAsync(bookRecommendationsDto.GoogleBooksId);
        if (book != null) return Result<Book>.Success(book);

        return await GetAndStoreEmbeddingsForBookAsync(bookRecommendationsDto);
    }

    private async Task<Result<List<BookRecommendationDto>>> GetRecommendationsAsync(Book book)
    {
        var recommendationsResult = await bookSearchService.GetNearestNeighbors(book);

        if (!recommendationsResult.IsSuccess)
            return Result<List<BookRecommendationDto>>.Failure(recommendationsResult.Error);

        var recommendations = recommendationsResult.Data
            .GroupBy(r => r.Title)
            .Select(g => g.First())
            .ToList();

        var recommendationsWithEmptyThumbnails = recommendations.Where(r => string.IsNullOrEmpty(r.Thumbnail));
        foreach (var recommendation in recommendationsWithEmptyThumbnails)
            recommendation.Thumbnail = configuration["DEFAULT_THUMBNAIL_URL"];


        return Result<List<BookRecommendationDto>>.Success(recommendations.Take(5).ToList());
    }


    private async Task<Result<Book>> GetAndStoreEmbeddingsForBookAsync(GetBookRecommendationsDto bookRecommendationsDto)
    {
        var embeddingRequestResult = bookEmbeddingService.ConstructEmbeddingRequest(bookRecommendationsDto);

        if (!embeddingRequestResult.IsSuccess) return Result<Book>.Failure(embeddingRequestResult.Error);

        var embedding = await bookEmbeddingService.GetEmbeddingsFromOpenAI(embeddingRequestResult.Data);
        var book = mapper.Map<Book>(bookRecommendationsDto);
        book.Embeddings = embedding.ToArray();

        await bookRepository.StoreBookAsync(book);
        return Result<Book>.Success(book);
    }
}