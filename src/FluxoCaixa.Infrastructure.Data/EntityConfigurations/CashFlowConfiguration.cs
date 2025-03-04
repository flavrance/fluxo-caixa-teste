using FluxoCaixa.Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FluxoCaixa.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// Configuração da entidade CashFlow para o Entity Framework
    /// </summary>
    public class CashFlowConfiguration : IEntityTypeConfiguration<CashFlow>
    {
        public void Configure(EntityTypeBuilder<CashFlow> builder)
        {
            builder.ToTable("CashFlows");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Date)
                .IsRequired();

            builder.Property(e => e.Balance)
                .HasPrecision(18, 2);

            // Remover o mapeamento direto da propriedade Entries que usa a interface IEntry
            // builder.HasMany(e => e.Entries)
            //    .WithOne()
            //    .HasForeignKey("CashFlowId")
            //    .OnDelete(DeleteBehavior.Cascade);

            // Configurar relacionamentos específicos com implementações concretas
            builder.HasMany<Credit>()
                .WithOne()
                .HasForeignKey(c => c.CashFlowId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany<Debit>()
                .WithOne()
                .HasForeignKey(d => d.CashFlowId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany<Report>()
                .WithOne()
                .HasForeignKey(r => r.CashFlowId);
        }
    }
} 