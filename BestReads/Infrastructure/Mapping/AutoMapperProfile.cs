using AutoMapper;
using BestReads.Core;
using BestReads.Features.BookRecommendations.Dtos;

namespace BestReads.Infrastructure.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<GetBookRecommendationsDto, Book>();
        CreateMap<Book, BookRecommendationDto>();
    }
}