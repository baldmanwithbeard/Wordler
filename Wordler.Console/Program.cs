//// See https://aka.ms/new-console-template for more information

using Wordler.Library.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wordler.ConsoleApp;

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