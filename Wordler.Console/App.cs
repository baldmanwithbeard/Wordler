using Wordler.Library.Messaging;

namespace Wordler.ConsoleApp;

internal class App
{
    private readonly IMessages _messages;

    public App(IMessages messages)
    {
        _messages = messages;
    }

    public void Run(string[] args)
    {
        var languageCode = args.FirstOrDefault(a => a.ToLower().StartsWith("lang="))?.Substring(5) ?? "en";
        var message = _messages.Greeting(languageCode);
        Console.WriteLine(message);
    }
}