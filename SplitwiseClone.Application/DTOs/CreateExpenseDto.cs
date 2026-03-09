namespace SplitwiseClone.Application.DTOs;

public class CreateExpenseDto
{
    public Guid GroupId { get; set; }
    public Guid PayerId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public List<Guid> ParticipantIds { get; set; } = new();
}