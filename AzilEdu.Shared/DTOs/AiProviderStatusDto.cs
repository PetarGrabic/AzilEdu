namespace AzilEdu.Shared.DTOs;

public class AiProviderStatusDto
{
    public string Provider { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public bool UsesExternalService { get; set; }
}
