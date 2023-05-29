using Wordler.Library;
using Wordler.Library.Messaging;

namespace Wordler.ConsoleApp;

internal class App
{
    private readonly IMessages _messages;
    private CommandFactory commandFactory;

    //private Game activeGame;
    private bool notBored = true;

    public App(IMessages messages)
    {
        commandFactory = new CommandFactory();
        _messages = messages;
        //activeGame = new Game();
    }

    public void Run()
    {
        Console.WriteLine("Welcome to Wordle Solver!");// + Environment.NewLine +
                                                       //"If this is your first time, please type \"help\", otherwise hit Enter to begin");
        Console.WriteLine("Hit Enter to begin!");
        if (Console.ReadLine()?.ToLower() == "help") ProvideHelp();
        while (notBored)
        {
            new Game().Run();
            Console.WriteLine("Anotha one? (y)");
            if (Console.ReadLine()?.ToLower() != "y") notBored = false;
        }
        //Console.WriteLine($"My first guess is: {activeGame.GetHighestScoreGuess()}");
        // Your code for Wordle solving logic goes here
        Console.WriteLine("Thank you for using Wordle Solver. Press any key to exit.");
        Console.ReadKey();
    }

    private static void ProvideHelp()
    {
        Console.WriteLine("");

        Console.ReadLine();
    }

    //for the last two, i might want an "Answer" class that keeps track of the solved positions in the word, as well as the letters than can be
    public void Run(IEnumerable<string> args)
    {
        var languageCode = args.FirstOrDefault(a => a.ToLower().StartsWith("lang="))?[5..] ?? "en";
        var message = _messages.Greeting(languageCode);
        Console.WriteLine(message);
    }
}