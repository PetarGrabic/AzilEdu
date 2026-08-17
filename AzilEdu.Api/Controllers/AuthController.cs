using AzilEdu.Api.Data;
using AzilEdu.Api.Security;
using AzilEdu.Shared.DTOs;
using AzilEdu.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzilEdu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AzilEduDbContext _context;
    private readonly JwtTokenService _tokenService;

    public AuthController(
        AzilEduDbContext context,
        JwtTokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _context.AppUsers
            .Include(item => item.UserRoles)
            .ThenInclude(item => item.AppRole)
            .FirstOrDefaultAsync(item => item.Email == email);

        if (user is null || !user.IsActive)
            return Unauthorized("Pogrešan email ili lozinka.");

        var hasher = new PasswordHasher<AppUser>();
        var result = hasher.VerifyHashedPassword(
            user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed)
            return Unauthorized("Pogrešan email ili lozinka.");

        var roles = user.UserRoles
            .Where(item => item.AppRole is not null)
            .Select(item => item.AppRole!.Name)
            .OrderBy(name => name)
            .ToList();

        return Ok(_tokenService.CreateLoginResponse(user, roles));
    }
}
