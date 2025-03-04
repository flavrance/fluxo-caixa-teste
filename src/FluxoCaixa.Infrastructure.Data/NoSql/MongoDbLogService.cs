using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluxoCaixa.Infrastructure.Data.NoSql
{
    public class LogEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Exception { get; set; } = string.Empty;
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }

    public interface ILogService
    {
        Task LogInformationAsync(string message, Dictionary<string, string>? properties = null);
        Task LogWarningAsync(string message, Dictionary<string, string>? properties = null);
        Task LogErrorAsync(string message, Exception? exception = null, Dictionary<string, string>? properties = null);
        Task<IEnumerable<LogEntry>> GetLogsAsync(DateTime startDate, DateTime endDate, string? level = null);
    }

    public class MongoDbLogService : ILogService
    {
        private readonly IMongoCollection<LogEntry> _logCollection;

        public MongoDbLogService(IMongoDatabase database)
        {
            _logCollection = database.GetCollection<LogEntry>("logs");
        }

        public async Task LogInformationAsync(string message, Dictionary<string, string>? properties = null)
        {
            await LogAsync("Information", message, null, properties);
        }

        public async Task LogWarningAsync(string message, Dictionary<string, string>? properties = null)
        {
            await LogAsync("Warning", message, null, properties);
        }

        public async Task LogErrorAsync(string message, Exception? exception = null, Dictionary<string, string>? properties = null)
        {
            await LogAsync("Error", message, exception, properties);
        }

        private async Task LogAsync(string level, string message, Exception? exception = null, Dictionary<string, string>? properties = null)
        {
            var logEntry = new LogEntry
            {
                Level = level,
                Message = message,
                Exception = exception?.ToString() ?? string.Empty,
                Properties = properties ?? new Dictionary<string, string>()
            };

            await _logCollection.InsertOneAsync(logEntry);
        }

        public async Task<IEnumerable<LogEntry>> GetLogsAsync(DateTime startDate, DateTime endDate, string? level = null)
        {
            var builder = Builders<LogEntry>.Filter;
            var filter = builder.Gte(x => x.Timestamp, startDate) & builder.Lte(x => x.Timestamp, endDate);

            if (!string.IsNullOrEmpty(level))
            {
                filter = filter & builder.Eq(x => x.Level, level);
            }

            return await _logCollection.Find(filter).SortByDescending(x => x.Timestamp).ToListAsync();
        }
    }
} 