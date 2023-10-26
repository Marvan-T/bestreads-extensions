using AutoMapper;
using BestReads.Core;
using BestReads.Core.Constants;
using BestReads.Core.Responses;
using BestReads.Features.BookRecommendations.Dtos;
using BestReads.Features.BookRecommendations.Repository;
using BestReads.Features.BookRecommendations.Services.BookEmbeddingService;

namespace BestReads.Features.BookRecommendations.Services.BookRecommendationService;

public class BookRecommendationService : IBookRecommendationService
{
    private readonly IBookEmbeddingService _bookEmbeddingService;
    private readonly IBookRepository _bookRepository;
    private readonly ILogger<BookRecommendationService> _logger;
    private readonly IMapper _mapper;

    public BookRecommendationService(IBookRepository bookRepository, IBookEmbeddingService bookEmbeddingService,
        IMapper mapper, ILogger<BookRecommendationService> logger)
    {
        _bookRepository = bookRepository;
        _bookEmbeddingService = bookEmbeddingService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ServiceResponse<List<BookRecommendationDto>>> GenerateRecommendations(
        GetBookRecommendationsDto bookRecommendationsDto)
    {
        var serviceResponse = new ServiceResponse<List<BookRecommendationDto>>();

        try
        {
            var book = await _bookRepository.GetByGoogleBooksIdAsync(bookRecommendationsDto.GoogleBooksId);

            if (book is null) book = await GetAndStoreEmbeddingsForBookAsync(bookRecommendationsDto);

            //Todo: obtain recommendations from Vector Search (bestreads-extensions #5)
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while generating recommendations for {GoogleBooksId}",
                bookRecommendationsDto.GoogleBooksId);
            serviceResponse.Success = false;
            serviceResponse.AddError(ErrorCodes.Feature_BookRecommendations.GenerateRecommendationsError, ex.Message);
        }

        return serviceResponse;
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
}