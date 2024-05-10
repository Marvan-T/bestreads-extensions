using BestReads.Core.Responses;
using BestReads.Features.BestSellers.Services.BestSellersService;
using BestReads.Infrastructure.ApiClients.NYTimes.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BestReads.Features.BestSellers.Controllers;

[ApiController]
[Route("api/extensions/[controller]")]
public class BestSellersController : ControllerBase
{
    private readonly IBestSellersService _bestSellersService;

    public BestSellersController(IBestSellersService bestSellersService)
    {
        _bestSellersService = bestSellersService;
    }

    [HttpGet]
    public async Task<ActionResult<ServiceResponse<BestSellerListDto>>> GetBestSellers()
    {
        var bestSellers = (await _bestSellersService.GetBestSellersAsync())
            .Match(
                ServiceResponse<BestSellerListDto>.Success,
                ServiceResponse<BestSellerListDto>.Failure
            );

        return bestSellers.Result.IsSuccess ? Ok(bestSellers) : BadRequest(bestSellers);
    }
}