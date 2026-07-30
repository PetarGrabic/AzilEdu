using AzilEdu.Api.Data;
using AzilEdu.Shared.DTOs;
using AzilEdu.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzilEdu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DonationsController : ControllerBase
{
    private readonly AzilEduDbContext _context;

    public DonationsController(AzilEduDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<DonationDto>>> GetDonations(
        [FromQuery] int? donorId,
        [FromQuery] int? typeId,
        [FromQuery] int? statusId,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo)
    {
        var query = _context.Donations
            .Include(donation => donation.Donor)
            .Include(donation => donation.DonationType)
            .Include(donation => donation.DonationStatus)
            .AsQueryable();

        if (donorId.HasValue)
            query = query.Where(donation => donation.DonorId == donorId.Value);

        if (typeId.HasValue)
            query = query.Where(donation => donation.DonationTypeId == typeId.Value);

        if (statusId.HasValue)
            query = query.Where(donation => donation.DonationStatusId == statusId.Value);

        if (dateFrom.HasValue)
            query = query.Where(donation => donation.DonationDate.Date >= dateFrom.Value.Date);

        if (dateTo.HasValue)
            query = query.Where(donation => donation.DonationDate.Date <= dateTo.Value.Date);

        var donations = await query
            .OrderByDescending(donation => donation.DonationDate)
            .ThenByDescending(donation => donation.Id)
            .ToListAsync();

        return Ok(donations.Select(ToDto).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DonationDto>> GetDonationById(int id)
    {
        var donation = await _context.Donations
            .Include(item => item.Donor)
            .Include(item => item.DonationType)
            .Include(item => item.DonationStatus)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (donation is null)
            return NotFound();

        return Ok(ToDto(donation));
    }

    [HttpPost]
    public async Task<ActionResult<DonationDto>> CreateDonation(SaveDonationDto request)
    {
        var validationError = ValidateDonation(request);

        if (validationError is not null)
            return BadRequest(validationError);

        var donation = new Donation
        {
            DonorId = request.DonorId,
            DonationTypeId = request.DonationTypeId,
            DonationStatusId = request.DonationStatusId,
            DonationDate = request.DonationDate,
            Amount = request.Amount,
            ItemName = request.ItemName,
            Quantity = request.Quantity,
            EstimatedValue = request.EstimatedValue,
            Notes = request.Notes
        };

        _context.Donations.Add(donation);
        await _context.SaveChangesAsync();

        await LoadReferences(donation);

        return CreatedAtAction(nameof(GetDonationById), new { id = donation.Id }, ToDto(donation));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDonation(int id, SaveDonationDto request)
    {
        var validationError = ValidateDonation(request);

        if (validationError is not null)
            return BadRequest(validationError);

        var donation = await _context.Donations.FindAsync(id);

        if (donation is null)
            return NotFound();

        donation.DonorId = request.DonorId;
        donation.DonationTypeId = request.DonationTypeId;
        donation.DonationStatusId = request.DonationStatusId;
        donation.DonationDate = request.DonationDate;
        donation.Amount = request.Amount;
        donation.ItemName = request.ItemName;
        donation.Quantity = request.Quantity;
        donation.EstimatedValue = request.EstimatedValue;
        donation.Notes = request.Notes;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDonation(int id)
    {
        var donation = await _context.Donations.FindAsync(id);

        if (donation is null)
            return NotFound();

        _context.Donations.Remove(donation);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task LoadReferences(Donation donation)
    {
        await _context.Entry(donation).Reference(item => item.Donor).LoadAsync();
        await _context.Entry(donation).Reference(item => item.DonationType).LoadAsync();
        await _context.Entry(donation).Reference(item => item.DonationStatus).LoadAsync();
    }

    private static string? ValidateDonation(SaveDonationDto request)
    {
        if (request.DonorId <= 0)
            return "Donator je obavezan.";

        if (request.DonationTypeId <= 0)
            return "Tip donacije je obavezan.";

        if (request.DonationStatusId <= 0)
            return "Status donacije je obavezan.";

        if (request.DonationDate.Date > DateTime.Today)
            return "Datum donacije ne smije biti u budućnosti.";

        if (request.Quantity.HasValue && request.Quantity.Value < 0)
            return "Količina ne smije biti negativna.";

        if (request.EstimatedValue.HasValue && request.EstimatedValue.Value < 0)
            return "Procijenjena vrijednost ne smije biti negativna.";

        var isMoneyDonation = request.DonationTypeId == 1;

        if (isMoneyDonation && (!request.Amount.HasValue || request.Amount.Value <= 0))
            return "Za novčanu donaciju potrebno je upisati iznos veći od nule.";

        if (!isMoneyDonation)
        {
            if (string.IsNullOrWhiteSpace(request.ItemName))
                return "Za materijalnu donaciju potrebno je upisati naziv donacije.";

            if (!request.Quantity.HasValue || request.Quantity.Value <= 0)
                return "Za materijalnu donaciju potrebno je upisati količinu veću od nule.";
        }

        return null;
    }

    private static DonationDto ToDto(Donation donation)
    {
        var donorName = donation.Donor is null
            ? string.Empty
            : !string.IsNullOrWhiteSpace(donation.Donor.OrganizationName)
                ? donation.Donor.OrganizationName
                : $"{donation.Donor.FirstName} {donation.Donor.LastName}".Trim();

        return new DonationDto
        {
            Id = donation.Id,
            DonorId = donation.DonorId,
            DonorName = donorName,
            DonationTypeId = donation.DonationTypeId,
            DonationType = donation.DonationType?.Name ?? string.Empty,
            DonationStatusId = donation.DonationStatusId,
            DonationStatus = donation.DonationStatus?.Name ?? string.Empty,
            DonationDate = donation.DonationDate,
            Amount = donation.Amount,
            ItemName = donation.ItemName,
            Quantity = donation.Quantity,
            EstimatedValue = donation.EstimatedValue,
            Notes = donation.Notes
        };
    }
}
