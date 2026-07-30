using AzilEdu.Api.Data;
using AzilEdu.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzilEdu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly AzilEduDbContext _context;

    public EmployeesController(AzilEduDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<EmployeeDto>>> GetEmployees()
    {
        var employees = await _context.Employees
            .Include(employee => employee.EmployeePosition)
            .Include(employee => employee.EmployeeStatus)
            .OrderBy(employee => employee.LastName)
            .ThenBy(employee => employee.FirstName)
            .Select(employee => new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                FullName = employee.FirstName + " " + employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                EmployeeNumber = employee.EmployeeNumber,
                HireDate = employee.HireDate,
                Notes = employee.Notes,
                EmployeePositionId = employee.EmployeePositionId,
                Position = employee.EmployeePosition != null ? employee.EmployeePosition.Name : "",
                EmployeeStatusId = employee.EmployeeStatusId,
                Status = employee.EmployeeStatus != null ? employee.EmployeeStatus.Name : ""
            })
            .ToListAsync();

        return Ok(employees);
    }

    [HttpGet("lookup")]
    public async Task<ActionResult<List<LookupDto>>> GetEmployeesLookup()
    {
        var employees = await _context.Employees
            .OrderBy(employee => employee.LastName)
            .ThenBy(employee => employee.FirstName)
            .Select(employee => new LookupDto
            {
                Id = employee.Id,
                Name = employee.FirstName + " " + employee.LastName
            })
            .ToListAsync();

        return Ok(employees);
    }
}
