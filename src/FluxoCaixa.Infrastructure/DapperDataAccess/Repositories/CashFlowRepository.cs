namespace FluxoCaixa.Infrastructure.DapperDataAccess.Repositories
{
    using Dapper;
    using FluxoCaixa.Application.Repositories;
    using FluxoCaixa.Domain.CashFlows;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;

    public class CashFlowRepository : ICashFlowReadOnlyRepository, ICashFlowWriteOnlyRepository
    {
        private readonly string _connectionString;

        public CashFlowRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task Add(CashFlow cashFlow, Credit credit)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string insertCashFlowSQL = "INSERT INTO CashFlow (Id, Year) VALUES (@Id, @Year)";

                DynamicParameters cashFlowParameters = new DynamicParameters();
                cashFlowParameters.Add("@id", cashFlow.Id);
                cashFlowParameters.Add("@year", cashFlow.Year);

                int cashFlowRows = await db.ExecuteAsync(insertCashFlowSQL, cashFlowParameters);
                
                string insertCreditSQL = "INSERT INTO [Credit] (Id, Amount, EntryDate, CashFlowId) " +
                    "VALUES (@Id, @Amount, @EntryDate, @CashFlowId)";

                DynamicParameters entryParameters = new DynamicParameters();
                entryParameters.Add("@id", credit.Id);
                entryParameters.Add("@amount", (double)credit.Amount);
                entryParameters.Add("@transactionDate", credit.EntryDate);
                entryParameters.Add("@cashFlowId", credit.CashFlowId);

                int creditRows = await db.ExecuteAsync(insertCreditSQL, entryParameters);
            }
        }

        public async Task Delete(CashFlow cashFlow)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string deleteSQL =
                    @"DELETE FROM [Credit] WHERE CashFlowId = @Id;
                      DELETE FROM [Debit] WHERE CashFlowId = @Id;
                      DELETE FROM CashFlow WHERE Id = @Id;";
                int rowsAffected = await db.ExecuteAsync(deleteSQL, cashFlow);
            }
        }

        public async Task<CashFlow> Get(Guid id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string cashFlowSQL = @"SELECT * FROM CashFlow WHERE Id = @Id";
                Entities.CashFlow cashFlow = await db
                    .QueryFirstOrDefaultAsync<Entities.CashFlow>(cashFlowSQL, new { id });

                if (cashFlow == null)
                    return null;

                string credits =
                    @"SELECT * FROM [Credit]
                      WHERE CashFlowId = @Id";

                List<IEntry> entriesList = new List<IEntry>();
                List<IEntry> reportList = new List<IEntry>();

                using (var reader = db.ExecuteReader(credits, new { id }))
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
                      WHERE CashFlowId = @Id";

                using (var reader = db.ExecuteReader(debits, new { id }))
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

                using (var reader = db.ExecuteReader(report, new { id }))
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
                return result;
            }
        }

        public async Task Update(CashFlow cashFlow, Credit credit)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string insertCreditSQL = "INSERT INTO [Credit] (Id, Amount, EntryDate, CashFlowId) " +
                    "VALUES (@Id, @Amount, @EntryDate, @CashFlowId)";

                DynamicParameters entryParameters = new DynamicParameters();
                entryParameters.Add("@id", credit.Id);
                entryParameters.Add("@amount", (double)credit.Amount);
                entryParameters.Add("@transactionDate", credit.EntryDate);
                entryParameters.Add("@cashFlowId", credit.CashFlowId);

                int creditRows = await db.ExecuteAsync(insertCreditSQL, entryParameters);
            }
        }

        public async Task Update(CashFlow cashFlow, Debit debit)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string insertDebitSQL = "INSERT INTO [Debit] (Id, Amount, EntryDate, CashFlowId) " +
                    "VALUES (@Id, @Amount, @EntryDate, @CashFlowId)";

                DynamicParameters entryParameters = new DynamicParameters();
                entryParameters.Add("@id", debit.Id);
                entryParameters.Add("@amount", (double)debit.Amount);
                entryParameters.Add("@transactionDate", debit.EntryDate);
                entryParameters.Add("@cashFlowId", debit.CashFlowId);

                int debitRows = await db.ExecuteAsync(insertDebitSQL, entryParameters);
            }
        }
    }
}
