using BestReads.Core.Utilities;
using BestReads.Features.BestSellers.Errors;
using BestReads.Infrastructure.ApiClients.NYTimes;
using BestReads.Infrastructure.ApiClients.NYTimes.DTOs;

namespace BestReads.Features.BestSellers.Services.BestSellersService;

public class BestSellersService(INYTimesApiClient nyTimesApiClient)
    : IBestSellersService
{
    private readonly INYTimesApiClient _nyTimesApiClient = nyTimesApiClient;

    public async Task<Result<BestSellerListDto>> GetBestSellersAsync()
    {
        var bestSellers = await _nyTimesApiClient.GetCurrentBestSellersList();
        return Result<BestSellerListDto>.Success(bestSellers); 
    }
}