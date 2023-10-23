using AutoMapper;
using BestReads.Core;
using BestReads.Core.Responses;
using BestReads.Features.BookRecommendations.Dtos;
using BestReads.Features.BookRecommendations.Repository;
using BestReads.Features.BookRecommendations.Services.BookEmbeddingService;

namespace BestReads.Features.BookRecommendations.Services.BookRecommendationService;

public class BookRecommendationService : IBookRecommendationService
{
    private readonly IBookEmbeddingService _bookEmbeddingService;
    private readonly IBookRepository _bookRepository;
    private readonly IMapper _mapper;

    public BookRecommendationService(IBookRepository bookRepository, IBookEmbeddingService bookEmbeddingService,
        IMapper mapper)
    {
        _bookRepository = bookRepository;
        _bookEmbeddingService = bookEmbeddingService;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<IList<BookRecommendationDto>>> GenerateRecommendations(
        GetBookRecommendationsDto bookRecommendationsDto)
    {
        var serviceRespones = new ServiceResponse<IList<BookRecommendationDto>>();
        var book = await _bookRepository.GetByGoogleBooksIdAsync(bookRecommendationsDto.GoogleBooksId);
        if (book is not null)
        {
            // obtain the book (incl embeddings) from DB 
        }
        else
        {
            // construct and save embedding request
            var embeddingRequest = _bookEmbeddingService.ConstructEmbeddingRequest(bookRecommendationsDto);
            var embedding = await _bookEmbeddingService.GetEmbeddingsFromOpenAI(embeddingRequest);
            book = _mapper.Map<Book>(bookRecommendationsDto);
            book.Embeddings = embedding.ToArray();
            await _bookRepository.StoreBookAsync(book);
        }
        // get the recommendations by calling vector search service.

        return serviceRespones;
    }
}