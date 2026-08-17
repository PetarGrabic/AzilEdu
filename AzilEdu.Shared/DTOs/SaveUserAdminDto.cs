namespace AzilEdu.Shared.DTOs;

public class SaveUserAdminDto
{
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Password { get; set; }
    public bool IsActive { get; set; } = true;
    public List<int> RoleIds { get; set; } = new();
    public int? VolunteerId { get; set; }
    public int? DonorId { get; set; }
    public int? EmployeeId { get; set; }
}
