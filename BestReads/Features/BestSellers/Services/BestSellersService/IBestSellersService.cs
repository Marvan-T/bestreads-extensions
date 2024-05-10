using BestReads.Core.Utilities;
using BestReads.Infrastructure.ApiClients.NYTimes.DTOs;

namespace BestReads.Features.BestSellers.Services.BestSellersService;

public interface IBestSellersService
{
    Task<Result<BestSellerListDto>> GetBestSellersAsync();
}