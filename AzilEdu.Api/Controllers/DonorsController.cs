using AzilEdu.Api.Data;
using AzilEdu.Shared.DTOs;
using AzilEdu.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzilEdu.Api.Controllers;

// DonorId će kasnije biti povezan s prijavljenim korisnikom preko AppUserId.
[ApiController]
[Route("api/[controller]")]
public class DonorsController : ControllerBase
{
    private readonly AzilEduDbContext _context;

    public DonorsController(AzilEduDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<DonorDto>>> GetDonors()
    {
        var donors = await _context.Donors
            .Include(donor => donor.DonorType)
            .Include(donor => donor.DonorStatus)
            .OrderBy(donor => donor.OrganizationName)
            .ThenBy(donor => donor.LastName)
            .ThenBy(donor => donor.FirstName)
            .Select(donor => new DonorDto
            {
                Id = donor.Id,
                FirstName = donor.FirstName,
                LastName = donor.LastName,
                OrganizationName = donor.OrganizationName,
                DisplayName = donor.OrganizationName != ""
                    ? donor.OrganizationName
                    : donor.FirstName + " " + donor.LastName,
                Email = donor.Email,
                Phone = donor.Phone,
                Address = donor.Address,
                City = donor.City,
                Notes = donor.Notes,
                CreatedAt = donor.CreatedAt,
                DonorTypeId = donor.DonorTypeId,
                Type = donor.DonorType != null ? donor.DonorType.Name : "",
                DonorStatusId = donor.DonorStatusId,
                Status = donor.DonorStatus != null ? donor.DonorStatus.Name : ""
            })
            .ToListAsync();

        return Ok(donors);
    }

    [HttpGet("lookup")]
    public async Task<ActionResult<List<LookupDto>>> GetDonorsLookup()
    {
        var donors = await _context.Donors
            .OrderBy(donor => donor.OrganizationName)
            .ThenBy(donor => donor.LastName)
            .ThenBy(donor => donor.FirstName)
            .Select(donor => new LookupDto
            {
                Id = donor.Id,
                Name = donor.OrganizationName != ""
                    ? donor.OrganizationName
                    : donor.FirstName + " " + donor.LastName
            })
            .ToListAsync();

        return Ok(donors);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DonorDto>> GetDonorById(int id)
    {
        var donor = await _context.Donors
            .Include(item => item.DonorType)
            .Include(item => item.DonorStatus)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (donor is null)
            return NotFound();

        return Ok(ToDto(donor));
    }

    [HttpPost]
    public async Task<ActionResult<DonorDto>> CreateDonor(SaveDonorDto dto)
    {
        var validationError = ValidateDonor(dto);

        if (validationError is not null)
        {
            return BadRequest(validationError);
        }

        var donor = new Donor
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            OrganizationName = dto.OrganizationName,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            City = dto.City,
            Notes = dto.Notes,
            CreatedAt = dto.CreatedAt,
            DonorTypeId = dto.DonorTypeId,
            DonorStatusId = dto.DonorStatusId
        };

        _context.Donors.Add(donor);
        await _context.SaveChangesAsync();

        var savedDonor = await _context.Donors
            .Include(item => item.DonorType)
            .Include(item => item.DonorStatus)
            .FirstAsync(item => item.Id == donor.Id);

        return CreatedAtAction(nameof(GetDonorById), new { id = donor.Id }, ToDto(savedDonor));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateDonor(int id, SaveDonorDto dto)
    {
        var validationError = ValidateDonor(dto);

        if (validationError is not null)
        {
            return BadRequest(validationError);
        }

        var donor = await _context.Donors.FindAsync(id);

        if (donor is null)
            return NotFound();

        donor.FirstName = dto.FirstName;
        donor.LastName = dto.LastName;
        donor.OrganizationName = dto.OrganizationName;
        donor.Email = dto.Email;
        donor.Phone = dto.Phone;
        donor.Address = dto.Address;
        donor.City = dto.City;
        donor.Notes = dto.Notes;
        donor.CreatedAt = dto.CreatedAt;
        donor.DonorTypeId = dto.DonorTypeId;
        donor.DonorStatusId = dto.DonorStatusId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static string? ValidateDonor(SaveDonorDto dto)
    {
        var hasOrganizationName = !string.IsNullOrWhiteSpace(dto.OrganizationName);
        var hasPersonName = !string.IsNullOrWhiteSpace(dto.FirstName) && !string.IsNullOrWhiteSpace(dto.LastName);

        if (!hasOrganizationName && !hasPersonName)
        {
            return "Unesi ime i prezime ili naziv organizacije.";
        }

        if (dto.DonorTypeId == 0)
        {
            return "Tip donatora je obavezan.";
        }

        if (dto.DonorStatusId == 0)
        {
            return "Status donatora je obavezan.";
        }

        return null;
    }

    private static DonorDto ToDto(Donor donor)
    {
        return new DonorDto
        {
            Id = donor.Id,
            FirstName = donor.FirstName,
            LastName = donor.LastName,
            OrganizationName = donor.OrganizationName,
            DisplayName = donor.OrganizationName != ""
                ? donor.OrganizationName
                : donor.FirstName + " " + donor.LastName,
            Email = donor.Email,
            Phone = donor.Phone,
            Address = donor.Address,
            City = donor.City,
            Notes = donor.Notes,
            CreatedAt = donor.CreatedAt,
            DonorTypeId = donor.DonorTypeId,
            Type = donor.DonorType != null ? donor.DonorType.Name : "",
            DonorStatusId = donor.DonorStatusId,
            Status = donor.DonorStatus != null ? donor.DonorStatus.Name : ""
        };
    }
}
