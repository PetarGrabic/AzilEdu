using AzilEdu.Api.Data;
using AzilEdu.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzilEdu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DonorStatusesController : ControllerBase
{
    private readonly AzilEduDbContext _context;

    public DonorStatusesController(AzilEduDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<LookupDto>>> GetDonorStatuses()
    {
        var result = await _context.DonorStatuses
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
