using AutoMapper;
using BestReads_Recommendations.Core;
using BestReads_Recommendations.Core.Responses;
using BestReads_Recommendations.Features.BookRecommendations.Dtos;
using BestReads_Recommendations.Features.BookRecommendations.Repository;
using BestReads_Recommendations.Features.BookRecommendations.Services.BookEmbeddingService;

namespace BestReads_Recommendations.Features.BookRecommendations.Services.BookRecommendationService;

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
            _bookRepository.Add(book);
            await _bookRepository.SaveChangesAsync();
        }

        // get the recommendations by calling vector search service.


        return null;
    }
    
    
    
}