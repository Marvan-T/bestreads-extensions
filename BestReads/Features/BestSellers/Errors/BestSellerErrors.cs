using BestReads.Core.Utilities;

namespace BestReads.Features.BestSellers.Errors;

public static class BestSellerErrors
{
    public static readonly Error FailedToRetrieveBestSellersList =
        new("best_seller_retrieval_error", "Failed to retrieve best sellers list");
}