using Microsoft.EntityFrameworkCore;
using SplitwiseClone.Application.DTOs;
using SplitwiseClone.Application.Interfaces;
using SplitwiseClone.Persistence.Data;

namespace SplitwiseClone.Application.Services;

public class BalanceService : IBalanceService
{
    private readonly AppDbContext _context;

    public BalanceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserBalanceDto>> GetGroupBalancesAsync(Guid groupId)
    {
        var balances = await _context.GroupMembers
            .Where(gm => gm.GroupId == groupId)
            .Select(gm => new UserBalanceDto
            {
                UserId = gm.UserId,
                UserName = gm.User!.UserName,
                Balance = _context.LedgerEntries
                    .Where(le => le.UserId == gm.UserId && le.Expense!.GroupId == groupId)
                    .Sum(le => (decimal?)le.Amount) ?? 0
            })
            .ToListAsync();

        return balances;
    }
}