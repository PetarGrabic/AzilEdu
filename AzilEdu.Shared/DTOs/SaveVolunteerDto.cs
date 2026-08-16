namespace AzilEdu.Shared.DTOs;

public class SaveVolunteerDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Skills { get; set; } = string.Empty;
    public DateTime? AvailableFrom { get; set; }
    public string Notes { get; set; } = string.Empty;
    public int VolunteerStatusId { get; set; }
}
