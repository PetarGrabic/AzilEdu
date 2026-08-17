using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AzilEdu.Shared.DTOs;
using AzilEdu.Shared.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AzilEdu.Api.Security;

public sealed class JwtTokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public LoginResponseDto CreateLoginResponse(
        AppUser user,
        IReadOnlyCollection<string> roles)
    {
        var now = DateTime.UtcNow;
        var expiresAtUtc = now.AddMinutes(_options.ExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.DisplayName),
            new(ClaimTypes.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        AddOptionalClaim(claims, AppClaimTypes.VolunteerId, user.VolunteerId);
        AddOptionalClaim(claims, AppClaimTypes.DonorId, user.DonorId);
        AddOptionalClaim(claims, AppClaimTypes.EmployeeId, user.EmployeeId);

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new LoginResponseDto
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAtUtc = expiresAtUtc,
            User = new LoggedUserDto
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName,
                Roles = roles.OrderBy(role => role).ToList(),
                VolunteerId = user.VolunteerId,
                DonorId = user.DonorId,
                EmployeeId = user.EmployeeId
            }
        };
    }

    private static void AddOptionalClaim(
        ICollection<Claim> claims,
        string claimType,
        int? value)
    {
        if (value.HasValue)
            claims.Add(new Claim(claimType, value.Value.ToString()));
    }
}
