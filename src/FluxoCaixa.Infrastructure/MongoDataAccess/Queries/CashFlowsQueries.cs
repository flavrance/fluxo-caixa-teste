namespace FluxoCaixa.Infrastructure.MongoDataAccess.Queries
{
    using MongoDB.Driver;
    using System;
    using System.Threading.Tasks;
    using FluxoCaixa.Application.Queries;
    using FluxoCaixa.Application.Results;
    using FluxoCaixa.Infrastructure.MongoDataAccess.Entities;
    using System.Collections.Generic;
    using System.Linq;

    public class CashFlowsQueries : ICashFlowsQueries
    {
        private readonly Context context;

        public CashFlowsQueries(Context context)
        {
            this.context = context;
        }

        public async Task<CashFlowResult> GetCashFlow(Guid cashFlowId)
        {
            CashFlow data = await context
                .CashFlows
                .Find(e => e.Id == cashFlowId)
                .SingleOrDefaultAsync();

            if (data == null)
                throw new CashFlowNotFoundException($"The cashFlow {cashFlowId} does not exists or is not processed yet.");

            List<Credit> credits = await context
                .Credits
                .Find(e => e.CashFlowId == cashFlowId)
                .ToListAsync();

            List<Debit> debits = await context
                .Debits
                .Find(e => e.CashFlowId == cashFlowId)
                .ToListAsync();

            double credit = credits.Sum(c => c.Amount);
            double debit = debits.Sum(d => d.Amount);

            List<EntryResult> entryResults = new List<EntryResult>();

            foreach (Credit entry in credits)
            {
                EntryResult entryResult = new EntryResult(
                    entry.Description, entry.Amount, entry.EntryDate);
                entryResults.Add(entryResult);
            }

            foreach (Debit entry in debits)
            {
                EntryResult entryResult = new EntryResult(
                    entry.Description, entry.Amount, entry.EntryDate);
                entryResults.Add(entryResult);
            }

            List<EntryResult> orderedEntries = entryResults.OrderBy(e => e.EntryDate).ToList();

            CashFlowResult cashFlowResult = new CashFlowResult(
                data.Id,
                credit - debit,
                orderedEntries);

            return cashFlowResult;
        }
    }
}
