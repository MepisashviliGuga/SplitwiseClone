using SplitwiseClone.Application.DTOs;
using SplitwiseClone.Application.Interfaces;
using SplitwiseClone.Domain.Entities;
using SplitwiseClone.Persistence.Data;
using SplitwiseClone.Application.Messages;
using MassTransit; // Add this
using Mapster;

namespace SplitwiseClone.Application.Services;

public class ExpenseService : IExpenseService
{
    private readonly AppDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint; // Add the Queue Publisher

    public ExpenseService(AppDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<bool> CreateExpenseAsync(CreateExpenseDto dto)
    {
        // 1. Save ONLY the Expense Header
        var expense = dto.Adapt<Expense>();
        expense.Id = Guid.NewGuid();
        expense.CreatedAt = DateTime.UtcNow;

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();

        // 2. Publish Event to the Queue! (This is instant)
        var expenseEvent = new ExpenseCreatedEvent
        {
            ExpenseId = expense.Id,
            GroupId = dto.GroupId,
            PayerId = dto.PayerId,
            TotalAmount = dto.TotalAmount,
            ParticipantIds = dto.ParticipantIds
        };

        await _publishEndpoint.Publish(expenseEvent);

        return true;
    }
}