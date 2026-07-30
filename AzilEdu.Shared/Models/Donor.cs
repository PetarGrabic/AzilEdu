namespace AzilEdu.Shared.Models;

public class Donor
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
    public DateTime CreatedAt { get; set; } = DateTime.Today;

    public int DonorTypeId { get; set; }
    public DonorType? DonorType { get; set; }

    public int DonorStatusId { get; set; }
    public DonorStatus? DonorStatus { get; set; }

    public ICollection<Donation> Donations { get; set; } = new List<Donation>();
}
