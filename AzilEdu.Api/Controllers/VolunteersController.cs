using AzilEdu.Api.Data;
using AzilEdu.Shared.DTOs;
using AzilEdu.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzilEdu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VolunteersController : ControllerBase
{
    private readonly AzilEduDbContext _context;

    public VolunteersController(AzilEduDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<VolunteerDto>>> GetVolunteers()
    {
        var volunteers = await _context.Volunteers
            .Include(volunteer => volunteer.VolunteerStatus)
            .OrderBy(volunteer => volunteer.LastName)
            .Select(volunteer => new VolunteerDto
            {
                Id = volunteer.Id,
                FirstName = volunteer.FirstName,
                LastName = volunteer.LastName,
                Email = volunteer.Email,
                Phone = volunteer.Phone,
                Skills = volunteer.Skills,
                AvailableFrom = volunteer.AvailableFrom,
                Notes = volunteer.Notes,
                VolunteerStatusId = volunteer.VolunteerStatusId,
                Status = volunteer.VolunteerStatus != null ? volunteer.VolunteerStatus.Name : string.Empty
            })
            .ToListAsync();

        return Ok(volunteers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VolunteerDto>> GetVolunteerById(int id)
    {
        var volunteer = await _context.Volunteers
            .Include(volunteer => volunteer.VolunteerStatus)
            .FirstOrDefaultAsync(volunteer => volunteer.Id == id);

        if (volunteer is null)
            return NotFound();

        var dto = new VolunteerDto
        {
            Id = volunteer.Id,
            FirstName = volunteer.FirstName,
            LastName = volunteer.LastName,
            Email = volunteer.Email,
            Phone = volunteer.Phone,
            Skills = volunteer.Skills,
            AvailableFrom = volunteer.AvailableFrom,
            Notes = volunteer.Notes,
            VolunteerStatusId = volunteer.VolunteerStatusId,
            Status = volunteer.VolunteerStatus != null ? volunteer.VolunteerStatus.Name : string.Empty
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<VolunteerDto>> CreateVolunteer(SaveVolunteerDto dto)
    {
        var volunteer = new Volunteer
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Skills = dto.Skills,
            AvailableFrom = dto.AvailableFrom,
            Notes = dto.Notes,
            VolunteerStatusId = dto.VolunteerStatusId
        };

        _context.Volunteers.Add(volunteer);
        await _context.SaveChangesAsync();

        var savedVolunteer = await _context.Volunteers
            .Include(v => v.VolunteerStatus)
            .FirstOrDefaultAsync(v => v.Id == volunteer.Id);

        if (savedVolunteer is null)
            return NotFound();

        var result = new VolunteerDto
        {
            Id = savedVolunteer.Id,
            FirstName = savedVolunteer.FirstName,
            LastName = savedVolunteer.LastName,
            Email = savedVolunteer.Email,
            Phone = savedVolunteer.Phone,
            Skills = savedVolunteer.Skills,
            AvailableFrom = savedVolunteer.AvailableFrom,
            Notes = savedVolunteer.Notes,
            VolunteerStatusId = savedVolunteer.VolunteerStatusId,
            Status = savedVolunteer.VolunteerStatus != null ? savedVolunteer.VolunteerStatus.Name : string.Empty
        };

        return CreatedAtAction(nameof(GetVolunteerById), new { id = volunteer.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVolunteer(int id, SaveVolunteerDto dto)
    {
        var volunteer = await _context.Volunteers.FindAsync(id);

        if (volunteer is null)
            return NotFound();

        volunteer.FirstName = dto.FirstName;
        volunteer.LastName = dto.LastName;
        volunteer.Email = dto.Email;
        volunteer.Phone = dto.Phone;
        volunteer.Skills = dto.Skills;
        volunteer.AvailableFrom = dto.AvailableFrom;
        volunteer.Notes = dto.Notes;
        volunteer.VolunteerStatusId = dto.VolunteerStatusId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVolunteer(int id)
    {
        var volunteer = await _context.Volunteers.FindAsync(id);

        if (volunteer is null)
            return NotFound();

        _context.Volunteers.Remove(volunteer);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
