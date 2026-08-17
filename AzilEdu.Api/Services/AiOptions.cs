namespace AzilEdu.Api.Services;

public sealed class AiOptions
{
    public const string SectionName = "Ai";

    public string Provider { get; set; } = "Mock";
    public string Model { get; set; } = "gpt-5.6-luna";
    public string ApiKey { get; set; } = string.Empty;
}
