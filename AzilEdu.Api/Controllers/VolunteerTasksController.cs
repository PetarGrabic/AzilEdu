using AzilEdu.Api.Data;
using AzilEdu.Shared.DTOs;
using AzilEdu.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzilEdu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VolunteerTasksController : ControllerBase
{
    private readonly AzilEduDbContext _context;

    public VolunteerTasksController(AzilEduDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<VolunteerTaskDto>>> GetVolunteerTasks(
        [FromQuery] int? statusId,
        [FromQuery] int? volunteerId,
        [FromQuery] int? animalId)
    {
        var query = _context.VolunteerTasks
            .Include(task => task.Volunteer)
            .Include(task => task.Animal)
            .Include(task => task.VolunteerTaskStatus)
            .Include(task => task.VolunteerTaskType)
            .AsQueryable();

        if (statusId.HasValue)
        {
            query = query.Where(task => task.VolunteerTaskStatusId == statusId.Value);
        }

        if (volunteerId.HasValue)
        {
            query = query.Where(task => task.VolunteerId == volunteerId.Value);
        }

        if (animalId.HasValue)
        {
            query = query.Where(task => task.AnimalId == animalId.Value);
        }

        var tasks = await query
            .OrderBy(task => task.DueDate)
            .ThenBy(task => task.Title)
            .ToListAsync();

        var result = tasks
            .Select(ToDto)
            .ToList();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VolunteerTaskDto>> GetVolunteerTaskById(int id)
    {
        var task = await _context.VolunteerTasks
            .Include(item => item.Volunteer)
            .Include(item => item.Animal)
            .Include(item => item.VolunteerTaskStatus)
            .Include(item => item.VolunteerTaskType)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (task is null)
        {
            return NotFound();
        }

        return Ok(ToDto(task));
    }

    [HttpPost]
    public async Task<ActionResult<VolunteerTaskDto>> CreateVolunteerTask(SaveVolunteerTaskDto request)
    {
        var task = new VolunteerTask
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            CompletedAt = request.CompletedAt,
            Notes = request.Notes,
            VolunteerId = request.VolunteerId,
            AnimalId = request.AnimalId,
            VolunteerTaskStatusId = request.VolunteerTaskStatusId,
            VolunteerTaskTypeId = request.VolunteerTaskTypeId
        };

        _context.VolunteerTasks.Add(task);
        await _context.SaveChangesAsync();

        var createdTask = await _context.VolunteerTasks
            .Include(item => item.Volunteer)
            .Include(item => item.Animal)
            .Include(item => item.VolunteerTaskStatus)
            .Include(item => item.VolunteerTaskType)
            .FirstAsync(item => item.Id == task.Id);

        return CreatedAtAction(
            nameof(GetVolunteerTaskById),
            new { id = task.Id },
            ToDto(createdTask));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVolunteerTask(int id, SaveVolunteerTaskDto request)
    {
        var task = await _context.VolunteerTasks.FindAsync(id);

        if (task is null)
        {
            return NotFound();
        }

        task.Title = request.Title;
        task.Description = request.Description;
        task.DueDate = request.DueDate;
        task.CompletedAt = request.CompletedAt;
        task.Notes = request.Notes;
        task.VolunteerId = request.VolunteerId;
        task.AnimalId = request.AnimalId;
        task.VolunteerTaskStatusId = request.VolunteerTaskStatusId;
        task.VolunteerTaskTypeId = request.VolunteerTaskTypeId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVolunteerTask(int id)
    {
        var task = await _context.VolunteerTasks.FindAsync(id);

        if (task is null)
        {
            return NotFound();
        }

        _context.VolunteerTasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static VolunteerTaskDto ToDto(VolunteerTask task)
    {
        return new VolunteerTaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            CompletedAt = task.CompletedAt,
            Notes = task.Notes,
            VolunteerId = task.VolunteerId,
            VolunteerName = task.Volunteer != null
                ? task.Volunteer.FirstName + " " + task.Volunteer.LastName
                : string.Empty,
            AnimalId = task.AnimalId,
            AnimalName = task.Animal != null ? task.Animal.Name : string.Empty,
            VolunteerTaskStatusId = task.VolunteerTaskStatusId,
            Status = task.VolunteerTaskStatus != null
                ? task.VolunteerTaskStatus.Name
                : string.Empty,
            VolunteerTaskTypeId = task.VolunteerTaskTypeId,
            Type = task.VolunteerTaskType != null
                ? task.VolunteerTaskType.Name
                : string.Empty
        };
    }
}