using AutoMapper;
using BestReads.Core;
using BestReads.Core.Constants;
using BestReads.Core.Responses;
using BestReads.Features.BookRecommendations.Dtos;
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

    public async Task<ServiceResponse<List<BookRecommendationDto>>> GenerateRecommendations(
        GetBookRecommendationsDto bookRecommendationsDto)
    {
        var serviceResponse = new ServiceResponse<List<BookRecommendationDto>>();
        _logger.LogInformation("Starting to generate book recommendations for GoogleBooksId: {GoogleBooksId}",
            bookRecommendationsDto.GoogleBooksId);

        try
        {
            var book = await GetOrStoreBookWithEmbeddingsAsync(bookRecommendationsDto);
            _logger.LogInformation("Book retrieved or stored with embeddings for GoogleBooksId: {GoogleBooksId}",
                book.GoogleBooksId);
            
            serviceResponse.Data = await GetRecommendationsAsync(book);
            _logger.LogInformation("Recommendations generated for GoogleBooksId: {GoogleBooksId}", book.GoogleBooksId);
        }
        catch (Exception ex)
        {
            HandleException(serviceResponse, bookRecommendationsDto.GoogleBooksId, ex);
        }

        return serviceResponse;
    }

    private async Task<Book> GetOrStoreBookWithEmbeddingsAsync(GetBookRecommendationsDto bookRecommendationsDto)
    {
        _logger.LogInformation("Attempting to get book by GoogleBooksId: {GoogleBooksId}",
            bookRecommendationsDto.GoogleBooksId);
        var book = await _bookRepository.GetByGoogleBooksIdAsync(bookRecommendationsDto.GoogleBooksId);

        if (book != null)
        {
            _logger.LogInformation("Found book with GoogleBooksId: {GoogleBooksId} in repository",
                bookRecommendationsDto.GoogleBooksId);
        }
        else
        {
            _logger.LogWarning(
                "Book with GoogleBooksId: {GoogleBooksId} not found in repository. Attempting to store with embeddings.",
                bookRecommendationsDto.GoogleBooksId);
            book = await GetAndStoreEmbeddingsForBookAsync(bookRecommendationsDto);
            _logger.LogInformation("Stored book with GoogleBooksId: {GoogleBooksId} and embeddings",
                bookRecommendationsDto.GoogleBooksId);
        }

        return book;
    }

    private async Task<List<BookRecommendationDto>> GetRecommendationsAsync(Book book)
    {
        _logger.LogInformation($"Generating Recommendations for {book.Title}");
        var recommendations = await _bookSearchService.GetNearestNeighbors(book);
        return recommendations.GroupBy(r => r.Title).Select(g => g.First()).Take(5).ToList();
    }

    private async Task<Book> GetAndStoreEmbeddingsForBookAsync(GetBookRecommendationsDto bookRecommendationsDto)
    {
        var embeddingRequest = _bookEmbeddingService.ConstructEmbeddingRequest(bookRecommendationsDto);
        var embedding = await _bookEmbeddingService.GetEmbeddingsFromOpenAI(embeddingRequest);

        var book = _mapper.Map<Book>(bookRecommendationsDto);
        book.Embeddings = embedding.ToArray();

        await _bookRepository.StoreBookAsync(book);
        return book;
    }

    private void HandleException(ServiceResponse<List<BookRecommendationDto>> serviceResponse, string googleBooksId,
        Exception ex)
    {
        _logger.LogError(ex, "Error while generating recommendations for {GoogleBooksId}", googleBooksId);
        serviceResponse.Success = false;
        serviceResponse.AddError(ErrorCodes.Feature_BookRecommendations.GenerateRecommendationsError, ex.Message);
    }
}