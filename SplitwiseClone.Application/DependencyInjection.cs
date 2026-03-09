using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using SplitwiseClone.Application.Interfaces;
using SplitwiseClone.Application.Services;
using System.Reflection;

namespace SplitwiseClone.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // 1. Setup Mapster
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        // 2. Register Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IBalanceService, BalanceService>();

        return services;
    }
}