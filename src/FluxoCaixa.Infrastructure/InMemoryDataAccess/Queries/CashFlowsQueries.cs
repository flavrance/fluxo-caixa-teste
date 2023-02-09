namespace FluxoCaixa.Infrastructure.InMemoryDataAccess.Queries
{
    using System;
    using System.Threading.Tasks;
    using FluxoCaixa.Application.Queries;
    using FluxoCaixa.Application.Results;
    using System.Collections.Generic;
    using System.Linq;
    using FluxoCaixa.Domain.CashFlows;

    public class CashFlowsQueries : ICashFlowsQueries
    {
        private readonly Context context;

        public CashFlowsQueries(Context context)
        {
            this.context = context;
        }

        public async Task<CashFlowResult> GetCashFlow(Guid cashFlowId)
        {
            CashFlow data = context
                .CashFlows
                .Where(e => e.Id == cashFlowId)
                .SingleOrDefault();

            if (data == null)
                throw new CashFlowNotFoundException($"The cashFlow {cashFlowId} does not exists or is not processed yet.");

            List<IEntry> credits = data
                .GetEntries()
                .Where(e => e is Credit)
                .ToList();

            List<IEntry> debits = data
                .GetEntries()
                .Where(e => e is Debit)
                .ToList();

            double credit = credits.Sum(c => c.Amount);
            double debit = debits.Sum(d => d.Amount);

            List<EntryResult> entryResults = new List<EntryResult>();
            List<EntryResult> reportResults = new List<EntryResult>();

            foreach (Credit entry in credits)
            {
                EntryResult entryResult = new EntryResult(
                    entry.Description, entry.Amount, entry.EntryDate);
                entryResults.Add(entryResult);
                reportResults.Add(entryResult);
            }

            foreach (Debit entry in debits)
            {
                EntryResult entryResult = new EntryResult(
                    entry.Description, entry.Amount, entry.EntryDate);
                entryResults.Add(entryResult);

                EntryResult reportResult = new EntryResult(
                    String.Empty, -entry.Amount, entry.EntryDate);
                reportResults.Add(reportResult);
            }

            List<EntryResult> orderedEntries = entryResults.OrderBy(e => e.EntryDate).ToList();
            List<EntryResult> reportEntries = reportResults.GroupBy(e => new { EntryDate = e.EntryDate }).Select(s => new EntryResult(String.Empty, s.Sum(x => x.Amount), s.Key.EntryDate)).ToList();

            CashFlowResult cashFlowResult = new CashFlowResult(
                data.Id,
                data.Year,
                credit - debit,
                orderedEntries,
                reportEntries);

            return await Task.FromResult<CashFlowResult>(cashFlowResult);
        }
    }
}
