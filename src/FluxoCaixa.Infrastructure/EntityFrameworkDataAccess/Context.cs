namespace FluxoCaixa.Infrastructure.EntityFrameworkDataAccess
{
    using Microsoft.EntityFrameworkCore;

    public class Context : DbContext
    {
        public Context(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Entities.CashFlow> CashFlows { get; set; }
        
        public DbSet<Entities.Credit> Credits { get; set; }
        public DbSet<Entities.Debit> Debits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.CashFlow>()
                .ToTable("CashFlow");            

            modelBuilder.Entity<Entities.Debit>()
                .ToTable("Debit");

            modelBuilder.Entity<Entities.Credit>()
                .ToTable("Credit");
        }
    }
}
