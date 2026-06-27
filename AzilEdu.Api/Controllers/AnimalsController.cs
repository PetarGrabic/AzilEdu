using AzilEdu.Api.Data;
using AzilEdu.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
 
namespace AzilEdu.Api.Controllers;
 
[ApiController]
[Route("api/[controller]")]
public class AnimalsController : ControllerBase
{
    private readonly AzilEduDbContext _context;
 
    public AnimalsController(AzilEduDbContext context)
    {
        _context = context;
    }
 
    [HttpGet]
    public async Task<ActionResult<List<Animal>>> GetAnimals()
    {
        var animals = await _context.Animals
            .OrderBy(animal => animal.Name)
            .ToListAsync();
 
        return Ok(animals);
    }
}