using AutoMapper;
using Azure.Search.Documents.Models;
using BestReads.Core;
using BestReads.Features.BookRecommendations.Dtos;

namespace BestReads.Infrastructure.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<GetBookRecommendationsDto, Book>();
        CreateMap<Book, BookRecommendationDto>();
        CreateMap<SearchDocument, BookRecommendationDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src["doc_id"]))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src["Title"]))
            .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src["Authors"]))
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src["Categories"]))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src["Description"]))
            .ForMember(dest => dest.Publisher, opt => opt.MapFrom(src => src["Publisher"]))
            .ForMember(dest => dest.PublishedDate, opt => opt.MapFrom(src => src["PublishedDate"]))
            .ForMember(dest => dest.Thumbnail, opt => opt.MapFrom(src => src["Thumbnail"]))
            .ForMember(dest => dest.GoogleBooksId, opt => opt.MapFrom(src => src["GoogleBooksId"]))
            .ForMember(dest => dest.IndustryIdentifiers, opt => opt.MapFrom(src => src["IndustryIdentifiers"]));
    }
}