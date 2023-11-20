using AutoMapper;
using BestReads.Core;
using BestReads.Core.Utilities;
using BestReads.Features.BookRecommendations.Dtos;
using BestReads.Features.BookRecommendations.Errors;
using BestReads.Features.BookRecommendations.Repository;
using BestReads.Features.BookRecommendations.Services.BookEmbeddingService;
using BestReads.Features.BookRecommendations.Services.BookSearchService;

namespace BestReads.Features.BookRecommendations.Services.BookRecommendationService;

public class BookRecommendationService : IBookRecommendationService
{
    private readonly IBookEmbeddingService _bookEmbeddingService;
    private readonly IBookRepository _bookRepository;
    private readonly IBookSearchService _bookSearchService;
    private readonly ILogger<BookRecommendationService> _logger;
    private readonly IMapper _mapper;

    public BookRecommendationService(IBookRepository bookRepository, IBookEmbeddingService bookEmbeddingService,
        IMapper mapper, ILogger<BookRecommendationService> logger, IBookSearchService bookSearchService)
    {
        _bookRepository = bookRepository;
        _bookEmbeddingService = bookEmbeddingService;
        _mapper = mapper;
        _logger = logger;
        _bookSearchService = bookSearchService;
    }

    public async Task<Result<List<BookRecommendationDto>>> GenerateRecommendations(
        GetBookRecommendationsDto bookRecommendationsDto)
    {
        try
        {
            var bookResult = await GetOrStoreBookWithEmbeddingsAsync(bookRecommendationsDto);
            if (!bookResult.IsSuccess) return Result<List<BookRecommendationDto>>.Failure(bookResult.Error);

            return await GetRecommendationsAsync(bookResult.Value);
        }
        catch (Exception ex)
        {
            return Result<List<BookRecommendationDto>>.Failure(new Error("UnexpectedError", ex.Message));
        }
    }


    private async Task<Result<Book>> GetOrStoreBookWithEmbeddingsAsync(GetBookRecommendationsDto bookRecommendationsDto)
    {
        if (string.IsNullOrEmpty(bookRecommendationsDto.GoogleBooksId))
            return Result<Book>.Failure(GenerateRecommendationErrors.GoogleBooksIdNotFound);

        var book = await _bookRepository.GetByGoogleBooksIdAsync(bookRecommendationsDto.GoogleBooksId);
        if (book != null) return Result<Book>.Success(book);

        return await GetAndStoreEmbeddingsForBookAsync(bookRecommendationsDto);
    }


    private async Task<Result<List<BookRecommendationDto>>> GetRecommendationsAsync(Book book)
    {
        var recommendationsResult = await _bookSearchService.GetNearestNeighbors(book);

        if (!recommendationsResult.IsSuccess)
            return Result<List<BookRecommendationDto>>.Failure(recommendationsResult.Error);

        var filteredRecommendations = recommendationsResult.Value.GroupBy(r => r.Title)
            .Select(g => g.First())
            .Take(5)
            .ToList();
        return Result<List<BookRecommendationDto>>.Success(filteredRecommendations);
    }


    private async Task<Result<Book>> GetAndStoreEmbeddingsForBookAsync(GetBookRecommendationsDto bookRecommendationsDto)
    {
        var embeddingRequestResult = _bookEmbeddingService.ConstructEmbeddingRequest(bookRecommendationsDto);

        if (!embeddingRequestResult.IsSuccess) return Result<Book>.Failure(embeddingRequestResult.Error);

        var embedding = await _bookEmbeddingService.GetEmbeddingsFromOpenAI(embeddingRequestResult.Value);
        var book = _mapper.Map<Book>(bookRecommendationsDto);
        book.Embeddings = embedding.ToArray();

        await _bookRepository.StoreBookAsync(book);
        return Result<Book>.Success(book);
    }
}