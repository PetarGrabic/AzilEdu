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
            .ThenBy(volunteer => volunteer.FirstName)
            .Select(volunteer => ToDto(volunteer))
            .ToListAsync();

        return Ok(volunteers);
    }

    [HttpGet("lookup")]
    public async Task<ActionResult<List<LookupDto>>> GetVolunteersLookup()
    {
        var volunteers = await _context.Volunteers
            .OrderBy(volunteer => volunteer.LastName)
            .ThenBy(volunteer => volunteer.FirstName)
            .Select(volunteer => new LookupDto
            {
                Id = volunteer.Id,
                Name = volunteer.FirstName + " " + volunteer.LastName
            })
            .ToListAsync();

        return Ok(volunteers);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VolunteerDto>> GetVolunteerById(int id)
    {
        var volunteer = await _context.Volunteers
            .Include(item => item.VolunteerStatus)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (volunteer is null)
            return NotFound();

        return Ok(ToDto(volunteer));
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
            VolunteerStatusId = dto.VolunteerStatusId == 0 ? 1 : dto.VolunteerStatusId
        };

        _context.Volunteers.Add(volunteer);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetVolunteerById), new { id = volunteer.Id }, volunteer);
    }

    [HttpPut("{id:int}")]
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
        volunteer.VolunteerStatusId = dto.VolunteerStatusId == 0 ? 1 : dto.VolunteerStatusId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteVolunteer(int id)
    {
        var volunteer = await _context.Volunteers.FindAsync(id);

        if (volunteer is null)
            return NotFound();

        _context.Volunteers.Remove(volunteer);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static VolunteerDto ToDto(Volunteer volunteer)
    {
        return new VolunteerDto
        {
            Id = volunteer.Id,
            FirstName = volunteer.FirstName,
            LastName = volunteer.LastName,
            FullName = volunteer.FirstName + " " + volunteer.LastName,
            Email = volunteer.Email,
            Phone = volunteer.Phone,
            Skills = volunteer.Skills,
            AvailableFrom = volunteer.AvailableFrom,
            Notes = volunteer.Notes,
            VolunteerStatusId = volunteer.VolunteerStatusId,
            Status = volunteer.VolunteerStatus != null ? volunteer.VolunteerStatus.Name : ""
        };
    }
}
