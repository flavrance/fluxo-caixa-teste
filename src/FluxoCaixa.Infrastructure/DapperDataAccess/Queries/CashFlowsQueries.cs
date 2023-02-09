namespace FluxoCaixa.Infrastructure.DapperDataAccess.Queries
{
    using Dapper;
    using FluxoCaixa.Domain.CashFlows;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using FluxoCaixa.Application.Queries;
    using FluxoCaixa.Application.Results;

    public class CashFlowsQueries : ICashFlowsQueries
    {
        private readonly string _connectionString;

        public CashFlowsQueries(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<CashFlowResult> GetCashFlow(Guid cashFlowId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string cashFlowtSQL = @"SELECT * FROM CashFlow WHERE Id = @cashFlowId";
                Entities.CashFlow cashFlow = await db
                    .QueryFirstOrDefaultAsync<Entities.CashFlow>(cashFlowtSQL, new { cashFlowId });

                if (cashFlow == null)
                    return null;

                string credits =
                    @"SELECT * FROM [Credit]
                      WHERE CashFlowId = @cashFlowId";

                List<IEntry> entriesList = new List<IEntry>();
                List<IEntry> reportList = new List<IEntry>();

                using (var reader = db.ExecuteReader(credits, new { cashFlowId }))
                {
                    var parser = reader.GetRowParser<Credit>();

                    while (reader.Read())
                    {
                        IEntry entry = parser(reader);
                        entriesList.Add(entry);
                    }
                }

                string debits =
                    @"SELECT * FROM [Debit]
                      WHERE CashFlowId = @cashFlowId";

                using (var reader = db.ExecuteReader(debits, new { cashFlowId }))
                {
                    var parser = reader.GetRowParser<Debit>();

                    while (reader.Read())
                    {
                        IEntry entry = parser(reader);
                        entriesList.Add(entry);
                    }
                }

                string report =
                    @"SELECT SUM(ISNULL(Credit.Amount,0) - ISNULL(Debit.Amount)) as Amount, '' as Description, ISNULL(Credit.EntryDate, Debit.EntryDate) as EntryDate 
                        FROM CashFlow 
                        LEFT JOIN Credit on CashFlow.Id = Credit.CashFlowId
                        LEFT JOIN Debit on CashFlow.Id = Debit.CashFlowId
                        WHERE CashFlow.Id = @cashFlowId
                        GROUP BY '', ISNULL(Credit.EntryDate, Debit.EntryDate)";                

                using (var reader = db.ExecuteReader(report, new { cashFlowId }))
                {
                    var parser = reader.GetRowParser<Report>();

                    while (reader.Read())
                    {
                        IEntry entry = parser(reader);
                        reportList.Add(entry);
                    }
                }

                EntryCollection entryCollection = new EntryCollection();

                foreach (var item in entriesList.OrderBy(e => e.EntryDate))
                {
                    entryCollection.Add(item);
                }

                ReportCollection reportCollection = new ReportCollection();

                foreach (var item in reportList.OrderBy(e => e.EntryDate))
                {
                    reportCollection.Add(item);
                }

                CashFlow result = CashFlow.Load(cashFlow.Id, cashFlow.Year, entryCollection, reportCollection);
                CashFlowResult cashFlowResult = new CashFlowResult(result);
                return cashFlowResult;
            }
        }
    }
}
