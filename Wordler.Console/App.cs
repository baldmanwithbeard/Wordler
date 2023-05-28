using Wordler.Library.Messaging;

namespace Wordler.ConsoleApp;

internal class App
{
    private readonly IMessages _messages;

    public App(IMessages messages)
    {
        _messages = messages;
    }

    public void Run(IEnumerable<string> args)
    {
        var languageCode = args.FirstOrDefault(a => a.ToLower().StartsWith("lang="))?[5..] ?? "en";
        var message = _messages.Greeting(languageCode);
        Console.WriteLine(message);
    }
}