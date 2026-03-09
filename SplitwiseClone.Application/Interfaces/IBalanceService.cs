using SplitwiseClone.Application.DTOs;

namespace SplitwiseClone.Application.Interfaces;

public interface IBalanceService
{
    Task<List<UserBalanceDto>> GetGroupBalancesAsync(Guid groupId);
}