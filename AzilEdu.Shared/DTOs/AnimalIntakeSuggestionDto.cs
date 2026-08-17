namespace AzilEdu.Shared.DTOs;

public class AnimalIntakeSuggestionDto
{
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string Breed { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public int? Age { get; set; }
    public DateTime? ArrivalDate { get; set; }
    public int AnimalStatusId { get; set; }
    public string Description { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public List<string> Warnings { get; set; } = new();
}
