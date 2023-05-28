//// See https://aka.ms/new-console-template for more information

using Wordler.Library.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wordler.ConsoleApp;

//Console.WriteLine("Welcome to Wordle Solver!");

//// Your code for Wordle solving logic goes here

//Console.WriteLine("Thank you for using Wordle Solver. Press any key to exit.");
//Console.ReadKey();

using var host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

try
{
    services.GetRequiredService<App>().Run(args);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

static IHostBuilder CreateHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) =>
        {
            services.AddSingleton<IMessages, Messages>();
            services.AddSingleton<App>();
        });
}