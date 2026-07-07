using AzilEdu.Api.Data;
using AzilEdu.Shared.DTOs;
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
    public async Task<ActionResult<List<AnimalDto>>> GetAnimals()
    {
        var animals = await _context.Animals
            .OrderBy(a => a.Name)
            .Select(a => new AnimalDto
            {
                Id = a.Id,
                Name = a.Name,
                Species = a.Species,
                Breed = a.Breed,
                Gender = a.Gender,
                Age = a.Age,
                ArrivalDate = a.ArrivalDate,
                IsAdopted = a.IsAdopted,
                ImageUrl = a.ImageUrl,
                Description = a.Description
            })
            .ToListAsync();
 
        return Ok(animals);
    }
}