using System.Text.Json;
using AzilEdu.Shared.DTOs;

namespace AzilEdu.Api.Services;

public class MockAiService : IAiService
{
    public string ProviderName => "Mock";
    public string ModelName => "Lokalni predvidljivi odgovori";
    public bool UsesExternalService => false;

    public Task<string> GenerateTextAsync(string purpose, string input)
    {
        var text = purpose switch
        {
            "animal-adoption" =>
                $"Upoznajte našeg štićenika! {input} Tražimo odgovoran i topao dom u kojem će dobiti sigurnost i pažnju.",
            "donor-thank-you" =>
                $"Hvala vam na podršci azilu. Vaša donacija izravno pomaže kvalitetnijoj brizi za životinje. {input}",
            "social-post" =>
                $"🐾 Traži se dom! {input} Podijelite objavu i pomozite nam pronaći pravu obitelj.",
            "daily-summary" =>
                $"Dnevni operativni sažetak: {input}",
            "volunteer-summary" =>
                $"Pregled zadataka prema hitnosti: {input}",
            _ =>
                $"AI prijedlog za svrhu '{purpose}': {input}"
        };

        return Task.FromResult(text);
    }

    public Task<T?> GenerateStructuredAsync<T>(string purpose, string input)
    {
        object? result = purpose switch
        {
            "animal-intake" => BuildAnimalIntake(input),
            "animal-data-check" => BuildAnimalDataCheck(input),
            _ => null
        };

        return Task.FromResult(result is T typedResult ? typedResult : default);
    }

    private static AnimalIntakeSuggestionDto BuildAnimalIntake(string input)
    {
        var lowerInput = input.ToLowerInvariant();

        return new AnimalIntakeSuggestionDto
        {
            Species = lowerInput.Contains("mačk") ? "Mačka" : "Pas",
            Gender = lowerInput.Contains("ženka") ? "Ženka" :
                     lowerInput.Contains("mužjak") || lowerInput.Contains("muški")
                         ? "Mužjak"
                         : string.Empty,
            ArrivalDate = DateTime.Today,
            AnimalStatusId = 1,
            Description = input.Trim(),
            Confidence = 0.65,
            Warnings = new List<string>
            {
                "Provjeri vrstu i spol.",
                "Ime, pasminu i starost dopuni ako nisu navedeni."
            }
        };
    }

    private static AnimalDataCheckDto BuildAnimalDataCheck(string input)
    {
        var warnings = new List<string>();
        using var document = JsonDocument.Parse(input);
        var root = document.RootElement;

        if (!root.TryGetProperty("Name", out var name) ||
            string.IsNullOrWhiteSpace(name.GetString()))
            warnings.Add("Ime nije uneseno.");

        if (!root.TryGetProperty("Species", out var species) ||
            string.IsNullOrWhiteSpace(species.GetString()))
            warnings.Add("Vrsta nije unesena.");

        if (root.TryGetProperty("Age", out var age) &&
            age.ValueKind == JsonValueKind.Number &&
            age.GetInt32() < 0)
            warnings.Add("Starost ne može biti negativna.");

        return new AnimalDataCheckDto
        {
            IsReady = warnings.Count == 0,
            Warnings = warnings,
            SuggestedDescription = warnings.Count == 0
                ? "Podaci su spremni za završni ljudski pregled."
                : "Prije spremanja ispravi navedena upozorenja."
        };
    }
}
