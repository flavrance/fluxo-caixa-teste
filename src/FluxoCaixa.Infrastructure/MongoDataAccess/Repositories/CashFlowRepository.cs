namespace FluxoCaixa.Infrastructure.MongoDataAccess.Repositories
{
    using FluxoCaixa.Domain.CashFlows;
    using FluxoCaixa.Application.Repositories;
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CashFlowRepository : ICashFlowReadOnlyRepository, ICashFlowWriteOnlyRepository
    {
        private readonly Context _context;

        public CashFlowRepository(Context context)
        {
            _context = context;
        }

        public async Task Add(CashFlow cashFlow, Credit credit)
        {
            Entities.CashFlow cashFlowEntity = new Entities.CashFlow()
            {                
                Id = cashFlow.Id
            };

            Entities.Credit creditEntity = new Entities.Credit()
            {
                CashFlowId = credit.CashFlowId,
                Amount = credit.Amount,
                Description = credit.Description,
                Id = credit.Id,
                EntryDate = credit.EntryDate
            };

            await _context.CashFlows.InsertOneAsync(cashFlowEntity);
            await _context.Credits.InsertOneAsync(creditEntity);
        }

        public async Task Delete(CashFlow cashFlow)
        {
            await _context.Credits.DeleteOneAsync(e => e.CashFlowId == cashFlow.Id);
            await _context.Debits.DeleteOneAsync(e => e.CashFlowId == cashFlow.Id);
            await _context.CashFlows.DeleteOneAsync(e => e.Id == cashFlow.Id);
        }

        public async Task<CashFlow> Get(Guid id)
        {
            Entities.CashFlow account = await _context
                .CashFlows
                .Find(e => e.Id == id)
                .SingleOrDefaultAsync();

            List<Entities.Credit> credits = await _context
                .Credits
                .Find(e => e.CashFlowId == id)
                .ToListAsync();

            List<Entities.Debit> debits = await _context
                .Debits
                .Find(e => e.CashFlowId == id)
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

            EntryCollection EntryCollection = new EntryCollection();
            EntryCollection.Add(orderedEntries);

            CashFlow result = CashFlow.Load(
                account.Id,
                EntryCollection);

            return result;
        }

        public async Task Update(CashFlow cashFlow, Credit credit)
        {
            Entities.Credit creditEntity = new Entities.Credit
            {
                CashFlowId = credit.CashFlowId,
                Amount = credit.Amount,
                Description = credit.Description,
                Id = credit.Id,
                EntryDate = credit.EntryDate
            };

            await _context.Credits.InsertOneAsync(creditEntity);
        }

        public async Task Update(CashFlow cashFlow, Debit debit)
        {
            Entities.Debit debitEntity = new Entities.Debit
            {
                CashFlowId = debit.CashFlowId,
                Amount = debit.Amount,
                Description = debit.Description,
                Id = debit.Id,
                EntryDate = debit.EntryDate
            };

            await _context.Debits.InsertOneAsync(debitEntity);
        }
    }
}
