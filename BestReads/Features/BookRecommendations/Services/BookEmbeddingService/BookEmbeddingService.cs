using BestReads.Core;
using BestReads.Core.Utilities;
using BestReads.Features.BookRecommendations.Dtos;
using BestReads.Features.BookRecommendations.Errors;

namespace BestReads.Features.BookRecommendations.Services.BookEmbeddingService;

public class BookEmbeddingService : IBookEmbeddingService
{
    private readonly IOpenAICleint _openAiCleint;

    public BookEmbeddingService(IOpenAICleint openAiCleint)
    {
        _openAiCleint = openAiCleint;
    }


    public async Task<IReadOnlyList<float>> GetEmbeddingsFromOpenAI(EmbeddingRequest request)
    {
        return await _openAiCleint.GetEmbeddingsAsync(request);
    }

    public Result<EmbeddingRequest> ConstructEmbeddingRequest(GetBookRecommendationsDto bookRecommendationsDto)
    {
        var validationResult = ValidateRecommendationsDto(bookRecommendationsDto);
        if (!validationResult.IsSuccess) return Result<EmbeddingRequest>.Failure(validationResult.Error);

        var embeddingText = $"Title: {bookRecommendationsDto.Title};" +
                            $"Authors: {string.Join(", ", bookRecommendationsDto.Authors)};" +
                            $"Categories: {string.Join(", ", bookRecommendationsDto.Categories)};" +
                            $"Description: {bookRecommendationsDto.Description};";

        var embeddingRequest = new EmbeddingRequest
        {
            Text = embeddingText
        };

        return Result<EmbeddingRequest>.Success(embeddingRequest);
    }

    private Result<GetBookRecommendationsDto> ValidateRecommendationsDto(GetBookRecommendationsDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return Result<GetBookRecommendationsDto>.Failure(GenerateRecommendationErrors.TitleRequired);

        if (dto.Authors == null || !dto.Authors.Any() || dto.Authors.Any(string.IsNullOrWhiteSpace))
            return Result<GetBookRecommendationsDto>.Failure(GenerateRecommendationErrors.AuthorsRequired);

        if (dto.Categories == null || !dto.Categories.Any() || dto.Categories.Any(string.IsNullOrWhiteSpace))
            return Result<GetBookRecommendationsDto>.Failure(GenerateRecommendationErrors.CategoriesRequired);

        if (string.IsNullOrWhiteSpace(dto.Description))
            return Result<GetBookRecommendationsDto>.Failure(GenerateRecommendationErrors.DescriptionRequired);

        return Result<GetBookRecommendationsDto>.Success(dto);
    }
}