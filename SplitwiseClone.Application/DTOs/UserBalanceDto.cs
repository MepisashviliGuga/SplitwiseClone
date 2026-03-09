namespace SplitwiseClone.Application.DTOs;

public class UserBalanceDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}