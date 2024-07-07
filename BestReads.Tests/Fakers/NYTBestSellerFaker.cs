using BestReads.Infrastructure.ApiClients.NYTimes.DTOs;
using Bogus;

namespace BestReads.Tests.Fakers;

public class NYTBestSellersFaker
{
    public class BookFaker : Faker<BookDto>
    {
        public BookFaker()
        {
            RuleFor(b => b.Rank, f => f.Random.Number(1, 100));
            RuleFor(b => b.Title, f => f.Lorem.Sentence(3));
            RuleFor(b => b.Author, f => f.Name.FullName());
            RuleFor(b => b.PrimaryIsbn10, f => f.Random.Replace("##########"));
            RuleFor(b => b.PrimaryIsbn13, f => f.Random.Replace("#############"));
            RuleFor(b => b.Thumbnail, f => f.Image.PicsumUrl());
        }
    }

    public class ListFaker : Faker<ListDto>
    {
        public ListFaker()
        {
            RuleFor(l => l.ListId, f => f.Random.Number(1, 100));
            RuleFor(l => l.ListName, f => f.Lorem.Word());
            RuleFor(
                l => l.DisplayName,
                f =>
                {
                    int count = f.Random.Number(1, 4);
                    List<string> words = f.Lorem.Words(count).ToList();
                    return string.Join(" ", words);
                }
            );
            RuleFor(
                l => l.Books,
                f => f.Make(f.Random.Number(5, 20), () => new BookFaker().Generate())
            );
        }
    }

    public class ResultFaker : Faker<ResultDto>
    {
        public ResultFaker()
        {
            RuleFor(r => r.Bestsellers_Date, f => f.Date.Recent().ToString("yyyy-MM-dd"));
            RuleFor(
                r => r.Lists,
                f => f.Make(f.Random.Number(1, 5), () => new ListFaker().Generate())
            );
        }
    }

    public class BestSellerListFaker : Faker<BestSellerListDto>
    {
        public BestSellerListFaker()
        {
            RuleFor(b => b.Results, f => new ResultFaker());
        }
    }

    public static BookDto GenerateBook() => new BookFaker().Generate();

    public static List<BookDto> GenerateBooks(int count) => new BookFaker().Generate(count);

    public static ListDto GenerateList() => new ListFaker().Generate();

    public static List<ListDto> GenerateLists(int count) => new ListFaker().Generate(count);

    public static ResultDto GenerateResult() => new ResultFaker().Generate();

    public static BestSellerListDto GenerateBestSellerList() =>
        new BestSellerListFaker().Generate();
}
