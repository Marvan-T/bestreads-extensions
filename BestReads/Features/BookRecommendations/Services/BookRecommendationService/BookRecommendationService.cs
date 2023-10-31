﻿using AutoMapper;
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

        try
        {
            var book = await _bookRepository.GetByGoogleBooksIdAsync(bookRecommendationsDto.GoogleBooksId);
            if (book is null) book = await GetAndStoreEmbeddingsForBookAsync(bookRecommendationsDto);
            serviceResponse.Data = await GetRecommendations(book);
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

    private async Task<List<BookRecommendationDto>> GetRecommendations(Book book)
    {
        var recommendations =
            await _bookSearchService.GetNearestNeighbors(book); //Returns a list of BookRecommendationDto
        return recommendations.GroupBy(r => r.Title)
            .Select(g => g.First())
            .Take(5)
            .ToList();
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