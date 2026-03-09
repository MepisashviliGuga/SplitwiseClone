using SplitwiseClone.Application.DTOs;

namespace SplitwiseClone.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
}