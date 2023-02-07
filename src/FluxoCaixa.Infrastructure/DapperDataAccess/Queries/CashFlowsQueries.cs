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

                EntryCollection entryCollection = new EntryCollection();

                foreach (var item in entriesList.OrderBy(e => e.EntryDate))
                {
                    entryCollection.Add(item);
                }

                CashFlow result = CashFlow.Load(cashFlow.Id, entryCollection);
                CashFlowResult cashFlowResult = new CashFlowResult(result);
                return cashFlowResult;
            }
        }
    }
}
