using AzilEdu.Api.Data;
using AzilEdu.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzilEdu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DonationTypesController : ControllerBase
{
    private readonly AzilEduDbContext _context;

    public DonationTypesController(AzilEduDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<LookupDto>>> GetDonationTypes()
    {
        var result = await _context.DonationTypes
            .OrderBy(type => type.Id)
            .Select(type => new LookupDto
            {
                Id = type.Id,
                Name = type.Name
            })
            .ToListAsync();

        return Ok(result);
    }
}
