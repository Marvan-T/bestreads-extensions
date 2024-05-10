using BestReads.Core.Utilities;
using BestReads.Features.BestSellers.Errors;
using BestReads.Infrastructure.ApiClients.NYTimes;
using BestReads.Infrastructure.ApiClients.NYTimes.DTOs;

namespace BestReads.Features.BestSellers.Services.BestSellersService;

public class BestSellersService(INYTimesApiClient nyTimesApiClient, ILogger<BestSellersService> bestSellerServiceLogger)
    : IBestSellersService
{
    private readonly ILogger<BestSellersService> _bestSellerServiceLogger = bestSellerServiceLogger;
    private readonly INYTimesApiClient _nyTimesApiClient = nyTimesApiClient;

    public async Task<Result<BestSellerListDto>> GetBestSellersAsync()
    {
        try
        {
            var bestSellers = await _nyTimesApiClient.GetCurrentBestSellersList();
            return Result<BestSellerListDto>.Success(bestSellers);
        }
        catch (Exception e)
        {
            // log the exception errors to a third party service when it's implemented 
            _bestSellerServiceLogger.LogError(e.Message);
            return Result<BestSellerListDto>.Failure(BestSellerErrors.FailedToRetrieveBestSellersList);
        }
    }
}