namespace AzilEdu.Shared.DTOs;

public class UserAdminDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<int> RoleIds { get; set; } = new();
    public List<string> Roles { get; set; } = new();
    public int? VolunteerId { get; set; }
    public string? VolunteerName { get; set; }
    public int? DonorId { get; set; }
    public string? DonorName { get; set; }
    public int? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
}
