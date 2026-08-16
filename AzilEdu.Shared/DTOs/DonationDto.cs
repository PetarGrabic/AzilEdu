namespace AzilEdu.Shared.DTOs;

public class DonationDto
{
    public int Id { get; set; }
    public int DonorId { get; set; }
    public string DonorName { get; set; } = string.Empty;
    public int DonationTypeId { get; set; }
    public string DonationType { get; set; } = string.Empty;
    public int DonationStatusId { get; set; }
    public string DonationStatus { get; set; } = string.Empty;
    public DateTime DonationDate { get; set; }
    public decimal? Amount { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal? Quantity { get; set; }
    public decimal? EstimatedValue { get; set; }
    public string Notes { get; set; } = string.Empty;
}