namespace SplitwiseClone.Domain.Entities;
public class LedgerEntry
{
    public Guid Id { get; set; }
    public Guid ExpenseId { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Expense? Expense { get; set; }

}