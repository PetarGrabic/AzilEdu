namespace AzilEdu.Shared.DTOs;

public class DonorDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int DonorTypeId { get; set; }
    public string Type { get; set; } = string.Empty;
    public int DonorStatusId { get; set; }
    public string Status { get; set; } = string.Empty;
}
