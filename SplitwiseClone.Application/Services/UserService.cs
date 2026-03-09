using Mapster;
using Microsoft.EntityFrameworkCore;
using SplitwiseClone.Application.DTOs;
using SplitwiseClone.Application.Interfaces;
using SplitwiseClone.Persistence.Data;

namespace SplitwiseClone.Application.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        return await _context.Users
            .ProjectToType<UserDto>()
            .ToListAsync();
    }
}