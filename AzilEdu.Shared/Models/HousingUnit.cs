namespace AzilEdu.Shared.Models;

public class HousingUnit
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string UnitType { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int Occupied { get; set; }
    public DateTime? LastCleanedAt { get; set; }
    public bool IsActive { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
}