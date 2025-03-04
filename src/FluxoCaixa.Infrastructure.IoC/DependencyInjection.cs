using FluxoCaixa.Application.Core.Interfaces.Services;
using FluxoCaixa.Application.Core.Services;
using FluxoCaixa.Domain.Core.Interfaces;
using FluxoCaixa.Domain.Core.Interfaces.Repositories;
using FluxoCaixa.Infrastructure.Data.Cache;
using FluxoCaixa.Infrastructure.Data.Context;
using FluxoCaixa.Infrastructure.Data.Messaging;
using FluxoCaixa.Infrastructure.Data.NoSql;
using FluxoCaixa.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using StackExchange.Redis;
using MongoDB.Driver;
using RabbitMQ.Client;
using System;

namespace FluxoCaixa.Infrastructure.IoC
{
    /// <summary>
    /// Configuração de injeção de dependência
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuração do banco de dados relacional
            services.AddDbContext<FluxoCaixaDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                var dbProvider = configuration.GetValue<string>("DatabaseSettings:Provider");

                if (dbProvider?.ToLower() == "postgres")
                {
                    options.UseNpgsql(connectionString, 
                        npgsqlOptions => npgsqlOptions.MigrationsAssembly("FluxoCaixa.Infrastructure.Data"));
                }
                else
                {
                    options.UseSqlServer(connectionString,
                        sqlOptions => sqlOptions.MigrationsAssembly("FluxoCaixa.Infrastructure.Data"));
                }
            });

            // Configuração do MongoDB
            services.AddSingleton<IMongoClient>(sp =>
            {
                var connectionString = configuration.GetConnectionString("MongoConnection");
                return new MongoClient(connectionString);
            });

            services.AddScoped(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                var url = new MongoUrl(configuration.GetConnectionString("MongoConnection") ?? "mongodb://localhost:27017/FluxoCaixa");
                var databaseName = url.DatabaseName;
                return client.GetDatabase(databaseName);
            });

            // Configuração do Redis
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var connectionString = configuration.GetConnectionString("RedisConnection") ?? "localhost:6379";
                return ConnectionMultiplexer.Connect(connectionString);
            });

            // Configuração do RabbitMQ
            services.AddSingleton<IConnectionFactory>(sp =>
            {
                var settings = configuration.GetSection("RabbitMQSettings");
                return new ConnectionFactory
                {
                    HostName = settings["HostName"] ?? "localhost",
                    UserName = settings["UserName"] ?? "guest",
                    Password = settings["Password"] ?? "guest",
                    VirtualHost = settings["VirtualHost"] ?? "/",
                    Port = int.Parse(settings["Port"] ?? "5672")
                };
            });

            // Configuração de serviços de infraestrutura
            services.Configure<RedisSettings>(configuration.GetSection("RedisSettings"));
            services.AddSingleton<ICacheService, RedisCacheService>();
            services.AddSingleton<IMessageService, RabbitMQService>();
            services.AddScoped<ILogService, MongoDbLogService>();

            // Registrar serviços da aplicação
            services.AddScoped<ICashFlowService, CashFlowService>();
            services.AddScoped<IReportService, ReportService>();

            // Registrar repositórios
            services.AddScoped<FluxoCaixa.Domain.Core.Interfaces.Repositories.ICashFlowReadOnlyRepository, CashFlowRepository>();
            services.AddScoped<FluxoCaixa.Domain.Core.Interfaces.Repositories.ICashFlowWriteOnlyRepository, CashFlowRepository>();

            // Registrar validadores
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Registrar MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            return services;
        }
    }
} 