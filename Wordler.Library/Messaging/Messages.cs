using System.Text.Json;
using Microsoft.Extensions.Logging;
using Wordler.Library.Models;

namespace Wordler.Library.Messaging;

public class Messages : IMessages
{
    private readonly ILogger<Messages> _log;

    public Messages(ILogger<Messages> log)
    {
        _log = log;
    }

    public string Greeting(string language) => LookUpCustomText("Greeting", language);

    private string LookUpCustomText(string key, string language)
    {
        JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };

        try
        {
            var messageSets = JsonSerializer.Deserialize<List<CustomText>>(File.ReadAllText("CustomText.json"), options);

            var messages = messageSets?.Where(x => x.Language == language).First();

            if (messages is null) throw new NullReferenceException("The specified language was not found in the json file.");

            return messages.Translations?[key] ?? string.Empty;
        }
        catch (Exception)
        {
            _log.LogError("Error looking up the custom text");
            throw;
        }
    }
}