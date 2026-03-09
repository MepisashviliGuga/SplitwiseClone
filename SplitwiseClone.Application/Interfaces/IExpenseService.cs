using SplitwiseClone.Application.DTOs;

namespace SplitwiseClone.Application.Interfaces;

public interface IExpenseService
{
    Task<bool> CreateExpenseAsync(CreateExpenseDto dto);
}