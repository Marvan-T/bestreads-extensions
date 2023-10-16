using AutoMapper;
using BestReads_Recommendations.Core;
using BestReads_Recommendations.Features.BookRecommendations.Dtos;

namespace BestReads_Recommendations.Infrastructure.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<GetBookRecommendationsDto, Book>();
        CreateMap<Book, BookRecommendationDto>();
    }
}