using FluxoCaixa.Infrastructure.IoC;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using FluxoCaixa.API.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using FluxoCaixa.Infrastructure.Data.Context;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/fluxo-caixa-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Fluxo de Caixa API",
        Version = "v1",
        Description = "API para gerenciamento de fluxo de caixa, permitindo o registro de créditos e débitos, além da geração de relatórios diários e por período.",
        Contact = new OpenApiContact
        {
            Name = "Equipe de Desenvolvimento",
            Email = "dev@fluxocaixa.com",
            Url = new Uri("https://github.com/fluxocaixa")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });
    
    // Habilitar anotações do Swagger
    c.EnableAnnotations();
    
    // Incluir comentários XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    
    // Agrupar endpoints por controller
    c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
    
    // Ordenar actions
    c.OrderActionsBy(apiDesc => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.RelativePath}");
});

// Adicionar infraestrutura (DI, DbContext, etc.)
builder.Services.AddInfrastructure(builder.Configuration);

// Add Entity Framework and repository services
builder.Services.AddEntityFrameworkServices(builder.Configuration);

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configurar Health Checks
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoConnection") ?? "";
var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection") ?? "";
var rabbitMQHostName = builder.Configuration.GetConnectionString("RabbitMQConnection") ?? "localhost";

builder.Services.AddHealthChecks()
    .AddSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection") ?? "",
        name: "sqlserver",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "db", "sql", "sqlserver" })
    .AddNpgSql(
        connectionString: builder.Configuration.GetConnectionString("PostgresConnection") ?? "",
        name: "postgres",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "db", "sql", "postgres" })
    .AddMongoDb(
        mongodbConnectionString: mongoConnectionString,
        name: "mongodb",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "db", "nosql", "mongodb" })
    .AddRedis(
        redisConnectionString: redisConnectionString,
        name: "redis",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "cache", "redis" })
    .AddRabbitMQ(
        rabbitConnectionString: $"amqp://guest:guest@{rabbitMQHostName}:5672",
        name: "rabbitmq",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "messaging", "rabbitmq" });

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fluxo de Caixa API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Configurar endpoints de Health Check
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    AllowCachingResponses = false
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    AllowCachingResponses = false
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    AllowCachingResponses = false
});

try
{
    Log.Information("Iniciando a aplicação Fluxo de Caixa");
    
    // Aplicar migrações automaticamente
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<FluxoCaixaDbContext>();
        Log.Information("Aplicando migrações do banco de dados...");
        
        try
        {
            // Verificar se o banco de dados existe antes de tentar aplicar migrações
            if (dbContext.Database.CanConnect())
            {
                Log.Information("Conexão com o banco de dados estabelecida. Aplicando migrações...");
                dbContext.Database.Migrate();
                Log.Information("Migrações aplicadas com sucesso!");
            }
            else
            {
                Log.Warning("Não foi possível conectar ao banco de dados. Verifique a string de conexão.");
            }
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Erro ao aplicar migrações. Continuando a execução...");
        }
    }
    
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "A aplicação Fluxo de Caixa falhou ao iniciar");
}
finally
{
    Log.CloseAndFlush();
}
