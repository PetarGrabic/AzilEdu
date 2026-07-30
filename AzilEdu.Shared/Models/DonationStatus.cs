namespace AzilEdu.Shared.Models;

public class DonationStatus
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Donation> Donations { get; set; } = new List<Donation>();
}
