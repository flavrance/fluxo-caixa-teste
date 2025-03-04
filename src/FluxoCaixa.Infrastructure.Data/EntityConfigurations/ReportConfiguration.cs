using FluxoCaixa.Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FluxoCaixa.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// Configuração da entidade Report para o Entity Framework
    /// </summary>
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.ToTable("Reports");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.CashFlowId)
                .IsRequired();

            builder.Property(r => r.Date)
                .IsRequired();

            // Configuração do Value Object Balance
            builder.OwnsOne(r => r.Balance, a =>
            {
                a.Property<decimal>("_value")
                    .HasColumnName("BalanceValue")
                    .IsRequired();
            });

            // Relacionamento com CashFlow
            builder.HasOne<CashFlow>()
                .WithMany()
                .HasForeignKey(r => r.CashFlowId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ignorar a coleção de entradas, pois será carregada separadamente
            builder.Ignore(r => r.Entries);
        }
    }
}