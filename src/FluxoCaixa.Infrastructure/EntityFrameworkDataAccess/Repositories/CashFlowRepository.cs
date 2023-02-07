namespace FluxoCaixa.Infrastructure.EntityFrameworkDataAccess
{
    using FluxoCaixa.Application.Repositories;
    using FluxoCaixa.Domain.CashFlows;
    using System;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using System.Data.SqlClient;
    using System.Collections.Generic;
    using System.Linq;

    public class CashFlowRepository : ICashFlowReadOnlyRepository, ICashFlowWriteOnlyRepository
    {
        private readonly Context _context;

        public CashFlowRepository(Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task Add(CashFlow cashFlow, Credit credit)
        {
            Entities.CashFlow accountEntity = new Entities.CashFlow()
            {                
                Id = cashFlow.Id
            };

            Entities.Credit creditEntity = new Entities.Credit()
            {
                CashFlowId = credit.CashFlowId,
                Amount = credit.Amount,
                Id = credit.Id,
                EntryDate = credit.EntryDate
            };

            await _context.CashFlows.AddAsync(accountEntity);
            await _context.Credits.AddAsync(creditEntity);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(CashFlow cashFlow)
        {
            string deleteSQL =
                    @"DELETE FROM Credit WHERE CashFlowId = @Id;
                      DELETE FROM Debit WHERE CashFlowId = @Id;
                      DELETE FROM CashFlow WHERE Id = @Id;";

            var id = new SqlParameter("@Id", cashFlow.Id);

            int affectedRows = await _context.Database.ExecuteSqlCommandAsync(
                deleteSQL, id);
        }

        public async Task<CashFlow> Get(Guid id)
        {
            Entities.CashFlow cashFlow = await _context
                .CashFlows
                .FindAsync(id);

            List<Entities.Credit> credits = await _context
                .Credits
                .Where(e => e.CashFlowId == id)
                .ToListAsync();

            List<Entities.Debit> debits = await _context
                .Debits
                .Where(e => e.CashFlowId == id)
                .ToListAsync();

            List<IEntry> entries = new List<IEntry>();

            foreach (Entities.Credit entryData in credits)
            {
                Credit entry = Credit.Load(
                    entryData.Id,
                    entryData.CashFlowId,
                    entryData.Amount,
                    entryData.EntryDate);

                entries.Add(entry);
            }

            foreach (Entities.Debit entryData in debits)
            {
                Debit entry = Debit.Load(
                    entryData.Id,
                    entryData.CashFlowId,
                    entryData.Amount,
                    entryData.EntryDate);

                entries.Add(entry);
            }

            var orderedEntries = entries.OrderBy(o => o.EntryDate).ToList();

            EntryCollection entryCollection = new EntryCollection();
            entryCollection.Add(orderedEntries);

            CashFlow result = CashFlow.Load(
                cashFlow.Id,                
                entryCollection);

            return result;
        }

        public async Task Update(CashFlow cashFlow, Credit credit)
        {
            Entities.Credit creditEntity = new Entities.Credit
            {
                CashFlowId = credit.CashFlowId,
                Amount = credit.Amount,
                Id = credit.Id,
                EntryDate = credit.EntryDate
            };

            await _context.Credits.AddAsync(creditEntity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(CashFlow cashFlow, Debit debit)
        {
            Entities.Debit debitEntity = new Entities.Debit
            {
                CashFlowId = debit.CashFlowId,
                Amount = debit.Amount,
                Id = debit.Id,
                EntryDate = debit.EntryDate
            };

            await _context.Debits.AddAsync(debitEntity);
            await _context.SaveChangesAsync();
        }
    }
}
