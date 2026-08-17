using System.Security.Claims;
using AzilEdu.Api.Data;
using AzilEdu.Api.Security;
using AzilEdu.Shared.DTOs;
using AzilEdu.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzilEdu.Api.Controllers;

[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AzilEduDbContext _context;

    public UsersController(AzilEduDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserAdminDto>>> GetUsers()
    {
        var users = await UserQuery()
            .OrderBy(user => user.DisplayName)
            .ToListAsync();

        return Ok(users.Select(ToDto).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserAdminDto>> GetUserById(int id)
    {
        var user = await UserQuery().FirstOrDefaultAsync(item => item.Id == id);
        return user is null ? NotFound() : Ok(ToDto(user));
    }

    [HttpGet("roles")]
    public async Task<ActionResult<List<RoleDto>>> GetRoles()
    {
        var roles = await _context.AppRoles
            .OrderBy(role => role.Id)
            .Select(role => new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                DisplayName = role.DisplayName
            })
            .ToListAsync();

        return Ok(roles);
    }

    [HttpPost]
    public async Task<ActionResult<UserAdminDto>> CreateUser(SaveUserAdminDto request)
    {
        var validationError = await ValidateRequest(request, null, passwordRequired: true);
        if (validationError is not null)
            return BadRequest(validationError);

        var user = new AppUser
        {
            Email = NormalizeEmail(request.Email),
            DisplayName = request.DisplayName.Trim(),
            IsActive = request.IsActive,
            VolunteerId = request.VolunteerId,
            DonorId = request.DonorId,
            EmployeeId = request.EmployeeId
        };

        var hasher = new PasswordHasher<AppUser>();
        user.PasswordHash = hasher.HashPassword(user, request.Password!);
        user.UserRoles = request.RoleIds
            .Distinct()
            .Select(roleId => new AppUserRole { AppRoleId = roleId })
            .ToList();

        _context.AppUsers.Add(user);
        await _context.SaveChangesAsync();

        var created = await UserQuery().FirstAsync(item => item.Id == user.Id);
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, ToDto(created));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, SaveUserAdminDto request)
    {
        var user = await _context.AppUsers
            .Include(item => item.UserRoles)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (user is null)
            return NotFound();

        var validationError = await ValidateRequest(request, id, passwordRequired: false);
        if (validationError is not null)
            return BadRequest(validationError);

        if (IsCurrentUser(id))
        {
            var adminRoleId = await _context.AppRoles
                .Where(role => role.Name == "Admin")
                .Select(role => role.Id)
                .SingleAsync();

            if (!request.IsActive || !request.RoleIds.Contains(adminRoleId))
                return BadRequest("Ne možeš deaktivirati vlastiti račun ni ukloniti vlastitu Admin ulogu.");
        }

        user.Email = NormalizeEmail(request.Email);
        user.DisplayName = request.DisplayName.Trim();
        user.IsActive = request.IsActive;
        user.VolunteerId = request.VolunteerId;
        user.DonorId = request.DonorId;
        user.EmployeeId = request.EmployeeId;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            var hasher = new PasswordHasher<AppUser>();
            user.PasswordHash = hasher.HashPassword(user, request.Password);
        }

        _context.AppUserRoles.RemoveRange(user.UserRoles);
        user.UserRoles = request.RoleIds
            .Distinct()
            .Select(roleId => new AppUserRole
            {
                AppUserId = user.Id,
                AppRoleId = roleId
            })
            .ToList();

        await _context.SaveChangesAsync();
        return NoContent();
    }

    private IQueryable<AppUser> UserQuery()
    {
        return _context.AppUsers
            .Include(user => user.UserRoles)
                .ThenInclude(userRole => userRole.AppRole)
            .Include(user => user.Volunteer)
            .Include(user => user.Donor)
            .Include(user => user.Employee);
    }

    private async Task<string?> ValidateRequest(
        SaveUserAdminDto request,
        int? currentUserId,
        bool passwordRequired)
    {
        var email = NormalizeEmail(request.Email);

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            return "Upiši ispravnu e-mail adresu.";

        if (string.IsNullOrWhiteSpace(request.DisplayName))
            return "Prikazno ime je obavezno.";

        if (passwordRequired && string.IsNullOrWhiteSpace(request.Password))
            return "Lozinka je obavezna za novog korisnika.";

        if (!string.IsNullOrWhiteSpace(request.Password) && request.Password.Length < 8)
            return "Lozinka mora imati najmanje 8 znakova.";

        if (request.RoleIds.Count == 0)
            return "Korisnik mora imati najmanje jednu ulogu.";

        var duplicateEmail = await _context.AppUsers.AnyAsync(user =>
            user.Email == email && user.Id != currentUserId);

        if (duplicateEmail)
            return "Korisnik s tom e-mail adresom već postoji.";

        var distinctRoleIds = request.RoleIds.Distinct().ToList();
        var rolesCount = await _context.AppRoles
            .CountAsync(role => distinctRoleIds.Contains(role.Id));

        if (rolesCount != distinctRoleIds.Count)
            return "Odabrana je nepostojeća uloga.";

        var profileError = await ValidateProfileLinks(request, currentUserId);
        return profileError;
    }

    private async Task<string?> ValidateProfileLinks(
        SaveUserAdminDto request,
        int? currentUserId)
    {
        if (request.VolunteerId.HasValue)
        {
            if (!await _context.Volunteers.AnyAsync(item => item.Id == request.VolunteerId))
                return "Odabrani volonter ne postoji.";

            if (await _context.AppUsers.AnyAsync(user =>
                    user.VolunteerId == request.VolunteerId && user.Id != currentUserId))
                return "Odabrani volonter već je povezan s drugim računom.";
        }

        if (request.DonorId.HasValue)
        {
            if (!await _context.Donors.AnyAsync(item => item.Id == request.DonorId))
                return "Odabrani donator ne postoji.";

            if (await _context.AppUsers.AnyAsync(user =>
                    user.DonorId == request.DonorId && user.Id != currentUserId))
                return "Odabrani donator već je povezan s drugim računom.";
        }

        if (request.EmployeeId.HasValue)
        {
            if (!await _context.Employees.AnyAsync(item => item.Id == request.EmployeeId))
                return "Odabrani djelatnik ne postoji.";

            if (await _context.AppUsers.AnyAsync(user =>
                    user.EmployeeId == request.EmployeeId && user.Id != currentUserId))
                return "Odabrani djelatnik već je povezan s drugim računom.";
        }

        return null;
    }

    private bool IsCurrentUser(int id)
    {
        return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var currentId)
            && currentId == id;
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }

    private static UserAdminDto ToDto(AppUser user)
    {
        return new UserAdminDto
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            IsActive = user.IsActive,
            RoleIds = user.UserRoles.Select(item => item.AppRoleId).OrderBy(id => id).ToList(),
            Roles = user.UserRoles
                .Where(item => item.AppRole is not null)
                .Select(item => item.AppRole!.Name)
                .OrderBy(name => name)
                .ToList(),
            VolunteerId = user.VolunteerId,
            VolunteerName = user.Volunteer is null
                ? null
                : $"{user.Volunteer.FirstName} {user.Volunteer.LastName}".Trim(),
            DonorId = user.DonorId,
            DonorName = user.Donor is null
                ? null
                : !string.IsNullOrWhiteSpace(user.Donor.OrganizationName)
                    ? user.Donor.OrganizationName
                    : $"{user.Donor.FirstName} {user.Donor.LastName}".Trim(),
            EmployeeId = user.EmployeeId,
            EmployeeName = user.Employee is null
                ? null
                : $"{user.Employee.FirstName} {user.Employee.LastName}".Trim()
        };
    }
}
