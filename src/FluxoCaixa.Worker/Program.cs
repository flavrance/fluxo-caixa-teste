using FluxoCaixa.Infrastructure.IoC;
using FluxoCaixa.Worker;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/worker-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddHostedService<DailyConsolidationWorker>();

// Adicionar serviços de infraestrutura
builder.Services.AddInfrastructure(builder.Configuration);

// Configurar Serilog
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(dispose: true);

var host = builder.Build();
host.Run();
