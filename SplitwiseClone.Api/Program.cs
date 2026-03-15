using MassTransit;
using SplitwiseClone.Application.Consumers;
using Microsoft.EntityFrameworkCore;
using SplitwiseClone.Application;
using SplitwiseClone.Persistence.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var appInsightsConnection = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");
if (!string.IsNullOrEmpty(appInsightsConnection))
{
    builder.Services.AddApplicationInsightsTelemetry(options =>
    {
        options.ConnectionString = appInsightsConnection;
    });
}

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddApplication();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ExpenseCreatedConsumer>();

    var serviceBusConnection = builder.Configuration.GetConnectionString("ServiceBus");

    if (string.IsNullOrEmpty(serviceBusConnection))
    {
        // Local development - use in-memory
        x.UsingInMemory((context, cfg) =>
        {
            cfg.ConfigureEndpoints(context);
        });
    }
    else
    {
        // Production - use Azure Service Bus
        x.UsingAzureServiceBus((context, cfg) =>
        {
            cfg.Host(serviceBusConnection);
            cfg.ConfigureEndpoints(context);
        });
    }
});
var app = builder.Build();

// Configure the HTTP request pipeline.

// Swagger enabled in all environments for demo purposes
// In production, this should be wrapped in IsDevelopment() check
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
