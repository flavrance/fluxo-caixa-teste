using FluxoCaixa.Domain.Core.Interfaces.Repositories;
using FluxoCaixa.Application.Core.Interfaces.Services;
using FluxoCaixa.Application.Core.Services;
using FluxoCaixa.Infrastructure.Data.Context;
using FluxoCaixa.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FluxoCaixa.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<FluxoCaixaDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("FluxoCaixa.Infrastructure.Data")));

            // Add Repositories
            services.AddScoped<ICashFlowReadOnlyRepository, CashFlowRepository>();
            services.AddScoped<ICashFlowWriteOnlyRepository, CashFlowRepository>();

            // Add Services
            services.AddScoped<ICashFlowService, CashFlowService>();

            return services;
        }
    }
} 