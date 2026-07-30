using AzilEdu.Api.Data;
using AzilEdu.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzilEdu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VolunteerTaskStatusesController : ControllerBase
{
    private readonly AzilEduDbContext _context;

    public VolunteerTaskStatusesController(AzilEduDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<LookupDto>>> GetVolunteerTaskStatuses()
    {
        var result = await _context.VolunteerTaskStatuses
            .OrderBy(status => status.Id)
            .Select(status => new LookupDto
            {
                Id = status.Id,
                Name = status.Name
            })
            .ToListAsync();

        return Ok(result);
    }
}