using System.Text.Json;
using AzilEdu.Shared.DTOs;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace AzilEdu.Api.Services;

public sealed class OpenAiService : IAiService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly ChatClient _client;
    private readonly AiOptions _options;

    public OpenAiService(IOptions<AiOptions> options)
    {
        _options = options.Value;

        if (string.IsNullOrWhiteSpace(_options.ApiKey))
            throw new InvalidOperationException("Ai:ApiKey nije postavljen.");

        _client = new ChatClient(_options.Model, _options.ApiKey);
    }

    public string ProviderName => "OpenAI";
    public string ModelName => _options.Model;
    public bool UsesExternalService => true;

    public async Task<string> GenerateTextAsync(string purpose, string input)
    {
        List<ChatMessage> messages =
        [
            new SystemChatMessage(GetSystemPrompt(purpose)),
            new UserChatMessage(input)
        ];

        ChatCompletion completion = await _client.CompleteChatAsync(messages);
        var text = completion.Content.FirstOrDefault()?.Text;

        if (string.IsNullOrWhiteSpace(text))
            throw new InvalidOperationException("AI servis nije vratio tekstualni odgovor.");

        return text.Trim();
    }

    public async Task<T?> GenerateStructuredAsync<T>(string purpose, string input)
    {
        var format = GetStructuredFormat<T>(purpose);

        if (format is null)
            return default;

        List<ChatMessage> messages =
        [
            new SystemChatMessage(GetSystemPrompt(purpose)),
            new UserChatMessage(input)
        ];

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: format.Value.Name,
                jsonSchema: BinaryData.FromString(format.Value.Schema),
                jsonSchemaIsStrict: true)
        };

        ChatCompletion completion = await _client.CompleteChatAsync(messages, options);
        var json = completion.Content.FirstOrDefault()?.Text;

        if (string.IsNullOrWhiteSpace(json))
            return default;

        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }

    private static string GetSystemPrompt(string purpose)
    {
        return purpose switch
        {
            "animal-adoption" =>
                "Piši topao, istinit i sažet opis životinje za udomljavanje na hrvatskom. Ne izmišljaj podatke koji nisu navedeni.",
            "donor-thank-you" =>
                "Napiši kratku personaliziranu zahvalu donatoru na hrvatskom. Ne navodi osjetljive podatke i ne obećavaj protuuslugu.",
            "social-post" =>
                "Napiši kratak tekst za društvenu mrežu na hrvatskom s jasnim pozivom na odgovorno udomljavanje. Ne izmišljaj činjenice.",
            "daily-summary" =>
                "Pretvori zadane agregirane brojke u kratak operativni sažetak azila na hrvatskom. Istakni hitne stavke bez izmišljanja uzroka.",
            "volunteer-summary" =>
                "Sažmi zadane volonterske zadatke na hrvatskom prema roku i hitnosti. Ne dodaj zadatke koji nisu u ulazu.",
            "animal-intake" =>
                "Iz bilješke izdvoji samo izričito navedene podatke o životinji. Nepoznate tekstualne vrijednosti vrati kao prazan tekst, a nepoznatu dob i datum kao null. Status 1 znači dostupna za udomljenje. Confidence mora biti od 0 do 1. Vrati samo JSON prema zadanoj shemi.",
            "animal-data-check" =>
                "Provjeri potpunost i logičnost podataka o životinji. Ne mijenjaj činjenice. Vrati samo JSON prema zadanoj shemi.",
            _ =>
                "Odgovori jasno i sažeto na hrvatskom. Koristi samo podatke iz ulaza."
        };
    }

    private static (string Name, string Schema)? GetStructuredFormat<T>(string purpose)
    {
        if (typeof(T) == typeof(AnimalIntakeSuggestionDto) && purpose == "animal-intake")
        {
            return ("animal_intake", """
                {
                  "type": "object",
                  "properties": {
                    "Name": { "type": "string" },
                    "Species": { "type": "string" },
                    "Breed": { "type": "string" },
                    "Gender": { "type": "string" },
                    "Age": { "type": ["integer", "null"] },
                    "ArrivalDate": { "type": ["string", "null"] },
                    "AnimalStatusId": { "type": "integer" },
                    "Description": { "type": "string" },
                    "Confidence": { "type": "number" },
                    "Warnings": {
                      "type": "array",
                      "items": { "type": "string" }
                    }
                  },
                  "required": [
                    "Name", "Species", "Breed", "Gender", "Age", "ArrivalDate",
                    "AnimalStatusId", "Description", "Confidence", "Warnings"
                  ],
                  "additionalProperties": false
                }
                """);
        }

        if (typeof(T) == typeof(AnimalDataCheckDto) && purpose == "animal-data-check")
        {
            return ("animal_data_check", """
                {
                  "type": "object",
                  "properties": {
                    "IsReady": { "type": "boolean" },
                    "Warnings": {
                      "type": "array",
                      "items": { "type": "string" }
                    },
                    "SuggestedDescription": { "type": "string" }
                  },
                  "required": ["IsReady", "Warnings", "SuggestedDescription"],
                  "additionalProperties": false
                }
                """);
        }

        return null;
    }
}
