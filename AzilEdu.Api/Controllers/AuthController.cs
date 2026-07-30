using AzilEdu.Api.Data;
using AzilEdu.Shared.DTOs;
using AzilEdu.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzilEdu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AzilEduDbContext _context;

    public AuthController(AzilEduDbContext context)
    {
        _context = context;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoggedUserDto>> Login(LoginRequestDto request)
    {
        var user = await _context.AppUsers
            .Include(item => item.UserRoles)
            .ThenInclude(item => item.AppRole)
            .FirstOrDefaultAsync(item => item.Email == request.Email);

        if (user is null || !user.IsActive)
            return Unauthorized("Pogrešan email ili lozinka.");

        var hasher = new PasswordHasher<AppUser>();
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed)
            return Unauthorized("Pogrešan email ili lozinka.");

        var roles = user.UserRoles
            .Where(item => item.AppRole is not null)
            .Select(item => item.AppRole!.Name)
            .OrderBy(name => name)
            .ToList();

        return Ok(new LoggedUserDto
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Roles = roles,
            VolunteerId = user.VolunteerId,
            DonorId = user.DonorId,
            EmployeeId = user.EmployeeId
        });
    }
}
