using AzilEdu.Api.Data;
using AzilEdu.Shared.DTOs;
using AzilEdu.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzilEdu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HousingUnitsController : ControllerBase
{
    private readonly AzilEduDbContext _context;

    public HousingUnitsController(AzilEduDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<HousingUnitDto>>> GetHousingUnits()
    {
        var housingUnits = await _context.HousingUnits
            .OrderBy(unit => unit.Name)
            .Select(unit => new HousingUnitDto
            {
                Id = unit.Id,
                Name = unit.Name,
                UnitType = unit.UnitType,
                Capacity = unit.Capacity,
                Occupied = unit.Occupied,
                LastCleanedAt = unit.LastCleanedAt,
                IsActive = unit.IsActive,
                ImageUrl = unit.ImageUrl,
                Note = unit.Note
            })
            .ToListAsync();

        return Ok(housingUnits);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HousingUnitDto>> GetHousingUnitById(int id)
    {
        var unit = await _context.HousingUnits.FindAsync(id);

        if (unit is null)
            return NotFound();

        var dto = new HousingUnitDto
        {
            Id = unit.Id,
            Name = unit.Name,
            UnitType = unit.UnitType,
            Capacity = unit.Capacity,
            Occupied = unit.Occupied,
            LastCleanedAt = unit.LastCleanedAt,
            IsActive = unit.IsActive,
            ImageUrl = unit.ImageUrl,
            Note = unit.Note
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<HousingUnitDto>> CreateHousingUnit(SaveHousingUnitDto dto)
    {
        var unit = new HousingUnit
        {
            Name = dto.Name,
            UnitType = dto.UnitType,
            Capacity = dto.Capacity,
            Occupied = dto.Occupied,
            LastCleanedAt = dto.LastCleanedAt,
            IsActive = dto.IsActive,
            ImageUrl = dto.ImageUrl,
            Note = dto.Note
        };

        _context.HousingUnits.Add(unit);
        await _context.SaveChangesAsync();

        var result = new HousingUnitDto
        {
            Id = unit.Id,
            Name = unit.Name,
            UnitType = unit.UnitType,
            Capacity = unit.Capacity,
            Occupied = unit.Occupied,
            LastCleanedAt = unit.LastCleanedAt,
            IsActive = unit.IsActive,
            ImageUrl = unit.ImageUrl,
            Note = unit.Note
        };

        return CreatedAtAction(nameof(GetHousingUnitById), new { id = unit.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateHousingUnit(int id, SaveHousingUnitDto dto)
    {
        var unit = await _context.HousingUnits.FindAsync(id);

        if (unit is null)
            return NotFound();

        unit.Name = dto.Name;
        unit.UnitType = dto.UnitType;
        unit.Capacity = dto.Capacity;
        unit.Occupied = dto.Occupied;
        unit.LastCleanedAt = dto.LastCleanedAt;
        unit.IsActive = dto.IsActive;
        unit.ImageUrl = dto.ImageUrl;
        unit.Note = dto.Note;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHousingUnit(int id)
    {
        var unit = await _context.HousingUnits.FindAsync(id);

        if (unit is null)
            return NotFound();

        _context.HousingUnits.Remove(unit);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
