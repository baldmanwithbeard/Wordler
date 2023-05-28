//// Use ReSharper's "to top-level code to apply new .NET 6 top-level statements syntax
//// (this will use implicit declaration of the Program class)
//// See https://aka.ms/new-console-template for more information on the template

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wordler.Library.Messaging;

namespace Wordler.ConsoleApp;

internal static class Program
{
    internal static void Main(string[] args)
    {
        using var host = CreateHostBuilder(args).Build();
        using var scope = host.Services.CreateScope();

        var services = scope.ServiceProvider;

        try
        {
            if (args.Any())
            {
                services.GetRequiredService<App>().Run(args);
                return;
            }

            services.GetRequiredService<App>().Run();
        }
        // rethrows any exception from the app as a console message
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
    }
}