namespace FluxoCaixa.WebApi
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Autofac.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Serilog;
    using Serilog.Events;
    using System.IO;

    internal sealed class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                    .UseStartup<Startup>()
                    .ConfigureAppConfiguration((builderContext, config) =>
                    {
                        IWebHostEnvironment env = builderContext.HostingEnvironment;
                        config.AddJsonFile("autofac.json");
                        config.AddEnvironmentVariables();
                    })
                    .UseSerilog((hostingContext, loggerConfig) =>                    
                        loggerConfig.ReadFrom.Configuration(hostingContext.Configuration)
                        .MinimumLevel.Debug()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                        .Enrich.FromLogContext()
                        .WriteTo.File(Path.Combine(hostingContext.HostingEnvironment.ContentRootPath, $"logs/log-{System.DateTime.UtcNow}.log")),
                        preserveStaticLogger: false,
                        writeToProviders: true
                    )
                    .ConfigureServices(services => services.AddAutofac())
                    .Build();
        }
    }
}
