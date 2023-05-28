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
        var languageCode = "en";

        foreach (var arg in args)
        {
            if (!arg.ToLower().StartsWith("lang=")) continue;
            //if (arg.Length < 5) throw new ArgumentOutOfRangeException(nameof(args), "Invalid lang= argument; invalid index range.");
            languageCode = arg[5..];

            break;
        }

        var message = _messages.Greeting(languageCode);

        Console.WriteLine(message);
    }
}