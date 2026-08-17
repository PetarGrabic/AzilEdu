namespace AzilEdu.Shared.DTOs;

public class AnimalDataCheckDto
{
    public bool IsReady { get; set; }
    public List<string> Warnings { get; set; } = new();
    public string SuggestedDescription { get; set; } = string.Empty;
}
