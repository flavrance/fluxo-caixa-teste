using FluxoCaixa.Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FluxoCaixa.Infrastructure.Data.Context
{
    /// <summary>
    /// Contexto do Entity Framework para o banco de dados
    /// </summary>
    public class FluxoCaixaDbContext : DbContext
    {
        public DbSet<CashFlow> CashFlows { get; set; }
        public DbSet<Credit> Credits { get; set; }
        public DbSet<Debit> Debits { get; set; }
        public DbSet<Report> Reports { get; set; }

        public FluxoCaixaDbContext(DbContextOptions<FluxoCaixaDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração para CashFlow
            modelBuilder.Entity<CashFlow>(entity =>
            {
                entity.ToTable("CashFlows");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                    
                entity.Property(e => e.Date)
                    .IsRequired();
                    
                entity.Property(e => e.Balance)
                    .HasPrecision(18, 2)
                    .IsRequired();
                
                // Ignorar a propriedade Entries pois é baseada em interface
                entity.Ignore(e => e.Entries);
                
                // Configurar relacionamento com Credit
                entity.HasMany(e => e.Credits)
                    .WithOne()
                    .HasForeignKey(e => e.CashFlowId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Configurar relacionamento com Debit
                entity.HasMany(e => e.Debits)
                    .WithOne()
                    .HasForeignKey(e => e.CashFlowId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            // Configuração para Credit
            modelBuilder.Entity<Credit>(entity =>
            {
                entity.ToTable("Credits");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.CashFlowId)
                    .IsRequired();
                    
                entity.Property(e => e.Amount)
                    .HasPrecision(18, 2)
                    .IsRequired();
                    
                entity.Property(e => e.Description)
                    .HasMaxLength(200);
                    
                entity.Property(e => e.Date)
                    .IsRequired();
                
                // Configurar relacionamento com CashFlow
                entity.HasOne<CashFlow>()
                    .WithMany(c => c.Credits)
                    .HasForeignKey(e => e.CashFlowId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            // Configuração para Debit
            modelBuilder.Entity<Debit>(entity =>
            {
                entity.ToTable("Debits");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.CashFlowId)
                    .IsRequired();
                    
                entity.Property(e => e.Amount)
                    .HasPrecision(18, 2)
                    .IsRequired();
                    
                entity.Property(e => e.Description)
                    .HasMaxLength(200);
                    
                entity.Property(e => e.Date)
                    .IsRequired();
                
                // Configurar relacionamento com CashFlow
                entity.HasOne<CashFlow>()
                    .WithMany(c => c.Debits)
                    .HasForeignKey(e => e.CashFlowId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Aplicar configurações de todas as classes de configuração no assembly atual
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
} 