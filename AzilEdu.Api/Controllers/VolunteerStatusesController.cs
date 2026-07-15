using AzilEdu.Api.Data;
using AzilEdu.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzilEdu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VolunteerStatusesController : ControllerBase
{
    private readonly AzilEduDbContext _context;

    public VolunteerStatusesController(AzilEduDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<LookupDto>>> GetVolunteerStatuses()
    {
        var result = await _context.VolunteerStatuses
            .OrderBy(status => status.Name)
            .Select(status => new LookupDto
            {
                Id = status.Id,
                Name = status.Name
            })
            .ToListAsync();

        return Ok(result);
    }
}
