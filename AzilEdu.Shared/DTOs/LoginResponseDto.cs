namespace AzilEdu.Shared.DTOs;

public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public LoggedUserDto User { get; set; } = new();
}
