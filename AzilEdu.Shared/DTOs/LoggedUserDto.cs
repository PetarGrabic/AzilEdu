namespace AzilEdu.Shared.DTOs;

public class LoggedUserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public int? VolunteerId { get; set; }
    public int? DonorId { get; set; }
    public int? EmployeeId { get; set; }
}