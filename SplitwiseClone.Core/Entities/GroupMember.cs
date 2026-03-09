namespace SplitwiseClone.Domain.Entities;

public class GroupMember
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public Group? Group { get; set; }
    public User? User { get; set; }
}