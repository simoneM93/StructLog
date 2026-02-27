using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StructLog.Interfaces;
using StructLog.Models;
using System.Text.Json;

namespace StructLog
{
    public class StructLog<T>(
        ILogger<T> logger,
        IOptions<StructLogOptions> options,
        IEnumerable<ILogEnricher> enrichers) : IStructLog<T> where T : class
    {
        private readonly ILogger<T> _logger = logger;
        private readonly StructLogOptions _options = options.Value;
        private readonly IEnumerable<ILogEnricher> _enrichers = enrichers;

        public IDisposable BeginScope(IDictionary<string, object> context)
            => _logger.BeginScope(context);

        public void Error(string message, IEventCode eventCode, Exception? ex = null, object? data = null)
            => Log(LogLevel.Error, message, eventCode, ex, data);

        public void Info(string message, IEventCode eventCode, object? data = null)
            => Log(LogLevel.Information, message, eventCode, null, data);

        public void Warning(string message, IEventCode eventCode, object? data = null)
            => Log(LogLevel.Warning, message, eventCode, null, data);

        record LogJsonState(string Json);

        private void Log(LogLevel level, string message, IEventCode eventCode, Exception? exception, object? data)
        {
            var contextData = new Dictionary<string, object>();

            if (_options.EnableEnrichers)
                foreach (var enricher in _enrichers)
                    enricher.Enrich(contextData);

            var logEntry = new
            {
                EventCode = eventCode.Code,
                EventDescription = eventCode.Description,
                Message = message,
                Context = contextData,
                Data = data
            };

            string json = JsonSerializer.Serialize(logEntry, new JsonSerializerOptions { WriteIndented = true });

            using (_logger.BeginScope(contextData))
                _logger.Log(level, default, (object)null, exception, (_, __) => json);
        }
    }
}
