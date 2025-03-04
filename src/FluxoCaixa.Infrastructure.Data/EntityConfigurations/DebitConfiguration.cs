using FluxoCaixa.Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FluxoCaixa.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// Configuração da entidade Debit para o Entity Framework
    /// </summary>
    public class DebitConfiguration : IEntityTypeConfiguration<Debit>
    {
        public void Configure(EntityTypeBuilder<Debit> builder)
        {
            builder.ToTable("Debits");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.CashFlowId)
                .IsRequired();

            builder.Property(e => e.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(e => e.Description)
                .HasMaxLength(200);

            builder.Property(e => e.Date)
                .IsRequired();
        }
    }
} 