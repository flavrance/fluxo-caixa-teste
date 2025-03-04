using FluxoCaixa.Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FluxoCaixa.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// Configuração da entidade Credit para o Entity Framework
    /// </summary>
    public class CreditConfiguration : IEntityTypeConfiguration<Credit>
    {
        public void Configure(EntityTypeBuilder<Credit> builder)
        {
            builder.ToTable("Credits");
            
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