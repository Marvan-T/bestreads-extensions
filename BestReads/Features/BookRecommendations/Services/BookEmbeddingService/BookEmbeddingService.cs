using BestReads.Core;
using BestReads.Core.Utilities;
using BestReads.Features.BookRecommendations.Dtos;
using BestReads.Features.BookRecommendations.Errors;

namespace BestReads.Features.BookRecommendations.Services.BookEmbeddingService;

public class BookEmbeddingService(IOpenAIClient openAiClient) : IBookEmbeddingService
{
    public async Task<IReadOnlyList<float>> GetEmbeddingsFromOpenAI(EmbeddingRequest request)
    {
        return await openAiClient.GetEmbeddingsAsync(request);
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

        if (dto.Authors == null || !dto.Authors.Exists(author => !string.IsNullOrWhiteSpace(author)))
            return Result<GetBookRecommendationsDto>.Failure(GenerateRecommendationErrors.AuthorsRequired);

        if (dto.Categories == null || !dto.Categories.Exists(category => !string.IsNullOrWhiteSpace(category)))
            return Result<GetBookRecommendationsDto>.Failure(GenerateRecommendationErrors.CategoriesRequired);

        if (string.IsNullOrWhiteSpace(dto.Description))
            return Result<GetBookRecommendationsDto>.Failure(GenerateRecommendationErrors.DescriptionRequired);

        return Result<GetBookRecommendationsDto>.Success(dto);
    }
}