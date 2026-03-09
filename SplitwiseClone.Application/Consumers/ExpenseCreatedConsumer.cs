using MassTransit;
using Microsoft.EntityFrameworkCore;
using SplitwiseClone.Application.Messages;
using SplitwiseClone.Domain.Entities;
using SplitwiseClone.Persistence.Data;

namespace SplitwiseClone.Application.Consumers;

public class ExpenseCreatedConsumer : IConsumer<ExpenseCreatedEvent>
{
    private readonly AppDbContext _context;

    public ExpenseCreatedConsumer(AppDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<ExpenseCreatedEvent> context)
    {
        var message = context.Message;

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Calculate the split
            decimal share = message.TotalAmount / message.ParticipantIds.Count;

            // 2. Create Ledger Entries
            foreach (var userId in message.ParticipantIds)
            {
                var entry = new LedgerEntry
                {
                    Id = Guid.NewGuid(),
                    ExpenseId = message.ExpenseId,
                    UserId = userId,
                    Amount = (userId == message.PayerId) ? (message.TotalAmount - share) : -share,
                    CreatedAt = DateTime.UtcNow
                };

                _context.LedgerEntries.Add(entry);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // Console log so we can see it working in the background!
            Console.WriteLine($"[QUEUE] Successfully processed ledger for Expense: {message.ExpenseId}");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"[QUEUE] Error processing ledger: {ex.Message}");
            // MassTransit will automatically retry if it fails!
            throw;
        }
    }
}