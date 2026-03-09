namespace SplitwiseClone.Domain.Entities;
public class Group
{
    public Guid Id { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}