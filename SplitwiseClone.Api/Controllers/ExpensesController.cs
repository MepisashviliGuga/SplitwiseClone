using Microsoft.AspNetCore.Mvc;
using SplitwiseClone.Application.DTOs;
using SplitwiseClone.Application.Interfaces;

namespace SplitwiseClone.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpensesController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateExpenseDto dto)
    {
        var result = await _expenseService.CreateExpenseAsync(dto);
        if (result) return Ok("Expense added successfully");
        return BadRequest("Failed to add expense");
    }
}