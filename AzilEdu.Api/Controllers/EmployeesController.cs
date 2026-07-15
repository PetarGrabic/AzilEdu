using AzilEdu.Api.Data;
using AzilEdu.Shared.DTOs;
using AzilEdu.Shared.Models;
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
            .Select(employee => new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                EmployeeNumber = employee.EmployeeNumber,
                HireDate = employee.HireDate,
                Notes = employee.Notes,
                EmployeePositionId = employee.EmployeePositionId,
                Position = employee.EmployeePosition != null ? employee.EmployeePosition.Name : string.Empty,
                EmployeeStatusId = employee.EmployeeStatusId,
                Status = employee.EmployeeStatus != null ? employee.EmployeeStatus.Name : string.Empty
            })
            .ToListAsync();

        return Ok(employees);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDto>> GetEmployeeById(int id)
    {
        var employee = await _context.Employees
            .Include(employee => employee.EmployeePosition)
            .Include(employee => employee.EmployeeStatus)
            .FirstOrDefaultAsync(employee => employee.Id == id);

        if (employee is null)
            return NotFound();

        var dto = new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Phone = employee.Phone,
            EmployeeNumber = employee.EmployeeNumber,
            HireDate = employee.HireDate,
            Notes = employee.Notes,
            EmployeePositionId = employee.EmployeePositionId,
            Position = employee.EmployeePosition != null ? employee.EmployeePosition.Name : string.Empty,
            EmployeeStatusId = employee.EmployeeStatusId,
            Status = employee.EmployeeStatus != null ? employee.EmployeeStatus.Name : string.Empty
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> CreateEmployee(SaveEmployeeDto dto)
    {
        var employee = new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            EmployeeNumber = dto.EmployeeNumber,
            HireDate = dto.HireDate,
            Notes = dto.Notes,
            EmployeePositionId = dto.EmployeePositionId,
            EmployeeStatusId = dto.EmployeeStatusId
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        var savedEmployee = await _context.Employees
            .Include(e => e.EmployeePosition)
            .Include(e => e.EmployeeStatus)
            .FirstOrDefaultAsync(e => e.Id == employee.Id);

        if (savedEmployee is null)
            return NotFound();

        var result = new EmployeeDto
        {
            Id = savedEmployee.Id,
            FirstName = savedEmployee.FirstName,
            LastName = savedEmployee.LastName,
            Email = savedEmployee.Email,
            Phone = savedEmployee.Phone,
            EmployeeNumber = savedEmployee.EmployeeNumber,
            HireDate = savedEmployee.HireDate,
            Notes = savedEmployee.Notes,
            EmployeePositionId = savedEmployee.EmployeePositionId,
            Position = savedEmployee.EmployeePosition != null ? savedEmployee.EmployeePosition.Name : string.Empty,
            EmployeeStatusId = savedEmployee.EmployeeStatusId,
            Status = savedEmployee.EmployeeStatus != null ? savedEmployee.EmployeeStatus.Name : string.Empty
        };

        return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, SaveEmployeeDto dto)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee is null)
            return NotFound();

        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Email = dto.Email;
        employee.Phone = dto.Phone;
        employee.EmployeeNumber = dto.EmployeeNumber;
        employee.HireDate = dto.HireDate;
        employee.Notes = dto.Notes;
        employee.EmployeePositionId = dto.EmployeePositionId;
        employee.EmployeeStatusId = dto.EmployeeStatusId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee is null)
            return NotFound();

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
