namespace AzilEdu.Shared.Models;

public class Donation
{
    public int Id { get; set; }

    public int DonorId { get; set; }
    public Donor? Donor { get; set; }

    public int DonationTypeId { get; set; }
    public DonationType? DonationType { get; set; }

    public int DonationStatusId { get; set; }
    public DonationStatus? DonationStatus { get; set; }

    public DateTime DonationDate { get; set; } = DateTime.Today;
    public decimal? Amount { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal? Quantity { get; set; }
    public decimal? EstimatedValue { get; set; }
    public string Notes { get; set; } = string.Empty;
}