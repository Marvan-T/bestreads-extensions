using BestReads.Core;
using BestReads.Features.BookRecommendations.Dtos;
using Bogus;
using MongoDB.Bson;

namespace BestReads.Tests.Fakers;

public static class BookFakers
{
    public static Faker<Book> BookModelFaker()
    {
        return new Faker<Book>()
            .RuleFor(b => b.Id, f => ObjectId.GenerateNewId().ToString())
            .RuleFor(b => b.Title, f => string.Join(" ", f.Lorem.Words(f.Random.Number(4, 8))))
            .RuleFor(b => b.Authors, f => f.Make(1, () => f.Name.FullName()).ToList())
            .RuleFor(b => b.Categories, f => f.Make(1, () => f.Commerce.Department()).ToList())
            .RuleFor(b => b.Description, f => f.Lorem.Paragraph())
            .RuleFor(b => b.Publisher, f => f.Company.CompanyName())
            .RuleFor(b => b.PublishedDate, f => f.Date.Past(20))
            .RuleFor(b => b.Thumbnail, f => f.Internet.Avatar())
            .RuleFor(b => b.GoogleBooksId, f => f.Random.AlphaNumeric(10))
            .RuleFor(b => b.IndustryIdentifiers, f => f.Make(1, () =>
            {
                var types = new List<string> { "ISBN_10", "ISBN_13", "ASIN" };
                var selectedType = f.PickRandom(types);
                var identifier = selectedType switch
                {
                    "ISBN_10" => f.Random.ReplaceNumbers("##########"),
                    "ISBN_13" => f.Random.ReplaceNumbers("#############"),
                    "ASIN" => f.Random.AlphaNumeric(10)
                };

                return new IndustryIdentifier
                {
                    Type = selectedType,
                    Identifier = identifier
                };
            }).ToList())
            .RuleFor(b => b.Embeddings, f => Enumerable.Range(0, 10).Select(_ => f.Random.Float()).ToArray());
    }

    public static Faker<BookRecommendationDto> BookRecommendationDtoFaker()
    {
        return new Faker<BookRecommendationDto>()
            .RuleFor(br => br.Id, f => f.IndexGlobal) // Incremental unique id
            .RuleFor(br => br.Title, f => string.Join(" ", f.Lorem.Words(f.Random.Number(4, 8))))
            .RuleFor(br => br.Thumbnail, f => f.Internet.Avatar());
    }

    public static Faker<GetBookRecommendationsDto> GetBookRecommendationDtoFaker()
    {
        return new Faker<GetBookRecommendationsDto>()
            .RuleFor(b => b.Title, f => string.Join(" ", f.Lorem.Words(f.Random.Number(4, 8))))
            .RuleFor(b => b.Authors, f => f.Make(1, () => f.Name.FullName()).ToList())
            .RuleFor(b => b.Categories, f => f.Make(1, () => f.Commerce.Department()).ToList())
            .RuleFor(b => b.Description, f => f.Lorem.Paragraph())
            .RuleFor(b => b.Publisher, f => f.Company.CompanyName())
            .RuleFor(b => b.PublishedDate, f => f.Date.Past(20))
            .RuleFor(b => b.Thumbnail, f => f.Internet.Avatar())
            .RuleFor(b => b.GoogleBooksId, f => f.Random.AlphaNumeric(10))
            .RuleFor(b => b.IndustryIdentifiers, f => f.Make(1, () =>
            {
                var types = new List<string> { "ISBN_10", "ISBN_13", "ASIN" };
                var selectedType = f.PickRandom(types);
                var identifier = selectedType switch
                {
                    "ISBN_10" => f.Random.ReplaceNumbers("##########"),
                    "ISBN_13" => f.Random.ReplaceNumbers("#############"),
                    "ASIN" => f.Random.AlphaNumeric(10)
                };

                return new IndustryIdentifier
                {
                    Type = selectedType,
                    Identifier = identifier
                };
            }).ToList());
    }
}