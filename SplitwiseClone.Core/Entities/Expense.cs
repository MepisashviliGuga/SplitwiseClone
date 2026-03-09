namespace SplitwiseClone.Domain.Entities;
public class Expense
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public Guid PayerId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}