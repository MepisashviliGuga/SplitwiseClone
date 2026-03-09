using Microsoft.AspNetCore.Mvc;
using SplitwiseClone.Application.Interfaces;

namespace SplitwiseClone.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BalancesController : ControllerBase
{
    private readonly IBalanceService _balanceService;

    public BalancesController(IBalanceService balanceService)
    {
        _balanceService = balanceService;
    }
    [HttpGet("group/{groupId}")]
    public async Task<IActionResult> GetGroupBalances(Guid groupId)
    {
        var balances = await _balanceService.GetGroupBalancesAsync(groupId);
        return Ok(balances);
    }
}