using AzilEdu.Api.Data;
using AzilEdu.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzilEdu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private const int MonetaryDonationTypeId = 1;
    private const int CompletedVolunteerTaskStatusId = 4;

    private readonly AzilEduDbContext _context;

    public DashboardController(AzilEduDbContext context)
    {
        _context = context;
    }

    // Kasnije će admin vidjeti sve podatke, a ostale role samo svoj dio aplikacije.
    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
    {
        var today = DateTime.Today;

        var summary = new DashboardSummaryDto
        {
            AnimalsCount = await _context.Animals.CountAsync(),
            AvailableAnimalsCount = await _context.Animals.CountAsync(animal => animal.AnimalStatusId == 1),
            ActiveVolunteersCount = await _context.Volunteers.CountAsync(volunteer => volunteer.VolunteerStatusId == 2),
            OpenVolunteerTasksCount = await _context.VolunteerTasks.CountAsync(task => task.VolunteerTaskStatusId == 1),
            ActiveDonorsCount = await _context.Donors.CountAsync(donor => donor.DonorStatusId == 2),
            EmployeesCount = await _context.Employees.CountAsync(),

            DonationsCount = await _context.Donations.CountAsync(),
            PendingDonationsCount = await _context.Donations.CountAsync(donation => donation.DonationStatusId == 1),
            MoneyDonationsTotal = await _context.Donations
                .Where(donation => donation.DonationTypeId == MonetaryDonationTypeId)
                .SumAsync(donation => donation.Amount ?? 0m),
            EstimatedMaterialDonationsTotal = await _context.Donations
                .Where(donation => donation.DonationTypeId != MonetaryDonationTypeId)
                .SumAsync(donation => donation.EstimatedValue ?? 0m),
            OverdueVolunteerTasksCount = await _context.VolunteerTasks
                .CountAsync(task => task.DueDate.HasValue
                    && task.DueDate.Value.Date < today
                    && task.VolunteerTaskStatusId != CompletedVolunteerTaskStatusId)
        };

        return Ok(summary);
    }

    [HttpGet("recent-donations")]
    public async Task<ActionResult<List<RecentDonationDto>>> GetRecentDonations()
    {
        var donations = await _context.Donations
            .Include(donation => donation.Donor)
            .Include(donation => donation.DonationType)
            .OrderByDescending(donation => donation.DonationDate)
            .Take(5)
            .ToListAsync();

        var result = donations.Select(donation => new RecentDonationDto
        {
            Id = donation.Id,
            DonorName = donation.Donor != null
                ? (donation.Donor.OrganizationName != ""
                    ? donation.Donor.OrganizationName
                    : donation.Donor.FirstName + " " + donation.Donor.LastName)
                : string.Empty,
            DonationType = donation.DonationType != null ? donation.DonationType.Name : string.Empty,
            DonationDate = donation.DonationDate,
            Amount = donation.Amount,
            ItemName = donation.ItemName
        }).ToList();

        return Ok(result);
    }
}
