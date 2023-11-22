using BestReads.Core.Utilities;

namespace BestReads.Features.BookRecommendations.Errors;

public static class GenerateRecommendationErrors
{
    public static readonly Error GoogleBooksIdNotFound =
        new("google_books_id_not_found", "Google Books ID is not provided.");

    public static readonly Error TitleRequired = new("title_required", "Title is required to generate embeddings.");

    public static readonly Error AuthorsRequired =
        new("authors_required", "At least one author is required to generate embeddings.");

    public static readonly Error CategoriesRequired =
        new("categories_required", "At least one category is required to generate embeddings.");

    public static readonly Error DescriptionRequired =
        new("description_required", "Description is required to generate embeddings.");

    public static readonly Error RecommendationsNotFound =
        new("recommendations_not_found", "No recommendations could be found.");
}