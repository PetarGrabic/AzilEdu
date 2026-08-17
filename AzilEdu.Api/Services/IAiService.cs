namespace AzilEdu.Api.Services;

public interface IAiService
{
    string ProviderName { get; }
    string ModelName { get; }
    bool UsesExternalService { get; }

    Task<string> GenerateTextAsync(string purpose, string input);
    Task<T?> GenerateStructuredAsync<T>(string purpose, string input);
}
