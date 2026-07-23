using AzilEdu.Api.Data;
using AzilEdu.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzilEdu.Api.Controllers;

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
}
