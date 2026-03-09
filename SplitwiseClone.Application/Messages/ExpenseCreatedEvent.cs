namespace SplitwiseClone.Application.Messages;

public class ExpenseCreatedEvent
{
    public Guid ExpenseId { get; set; }
    public Guid GroupId { get; set; }
    public Guid PayerId { get; set; }
    public decimal TotalAmount { get; set; }
    public List<Guid> ParticipantIds { get; set; } = new();
}