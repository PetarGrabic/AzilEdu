namespace AzilEdu.Shared.DTOs;

public class RecentDonationDto
{
    public int Id { get; set; }
    public string DonorName { get; set; } = string.Empty;
    public string DonationType { get; set; } = string.Empty;
    public DateTime DonationDate { get; set; }
    public decimal? Amount { get; set; }
    public string ItemName { get; set; } = string.Empty;
}
