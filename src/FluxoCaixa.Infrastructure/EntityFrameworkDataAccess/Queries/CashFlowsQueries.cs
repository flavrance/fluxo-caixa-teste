namespace FluxoCaixa.Infrastructure.EntityFrameworkDataAccess.Queries
{
    using System;
    using System.Threading.Tasks;
    using FluxoCaixa.Application.Queries;
    using FluxoCaixa.Application.Results;
    using System.Collections.Generic;
    using System.Linq;
    using FluxoCaixa.Domain.CashFlows;
    using Microsoft.EntityFrameworkCore;

    public class CashFlowsQueries : ICashFlowsQueries
    {
        private readonly Context _context;

        public CashFlowsQueries(Context context)
        {
            _context = context;
        }

        public async Task<CashFlowResult> GetCashFlow(Guid cashFlowId)
        {
            Entities.CashFlow account = await _context
                .CashFlows
                .FindAsync(cashFlowId);

            List<Entities.Credit> credits = await _context
                .Credits
                .Where(e => e.CashFlowId == cashFlowId)
                .ToListAsync();

            List<Entities.Debit> debits = await _context
                .Debits
                .Where(e => e.CashFlowId == cashFlowId)
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

            var orderedTransactions = entries.OrderBy(o => o.EntryDate).ToList();

            EntryCollection entryCollection = new EntryCollection();
            entryCollection.Add(orderedTransactions);

            CashFlow result = CashFlow.Load(
                account.Id,                
                entryCollection);

            CashFlowResult re = new CashFlowResult(result);
            return re;
        }
    }
}
