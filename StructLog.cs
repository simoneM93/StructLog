using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StructLog.Interfaces;
using StructLog.Models;
using System.Text.Json;

namespace StructLog
{
    public class StructLog<T> : IStructLog<T> where T : class
    {
        private readonly ILogger<T> _logger;
        private readonly StructLogOptions _options;
        private readonly IEnumerable<ILogEnricher> _enrichers;
        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

        public StructLog(
            ILogger<T> logger,
            IOptions<StructLogOptions> options,
            IEnumerable<ILogEnricher> enrichers)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(enrichers);

            _logger = logger;
            _options = options.Value;
            _enrichers = enrichers;
        }

        public IDisposable? BeginScope(IDictionary<string, object> context)
            => _logger.BeginScope(context);

        public void Critical(string message, IEventCode eventCode, Exception? ex = null, object? data = null)
            => Log(LogLevel.Critical, message, eventCode, ex, data);

        public void Error(string message, IEventCode eventCode, Exception? ex = null, object? data = null)
            => Log(LogLevel.Error, message, eventCode, ex, data);

        public void Warning(string message, IEventCode eventCode, object? data = null)
            => Log(LogLevel.Warning, message, eventCode, null, data);

        public void Info(string message, IEventCode eventCode, object? data = null)
            => Log(LogLevel.Information, message, eventCode, null, data);

        public void Debug(string message, IEventCode eventCode, object? data = null)
            => Log(LogLevel.Debug, message, eventCode, null, data);

        public void Trace(string message, IEventCode eventCode, object? data = null)
            => Log(LogLevel.Trace, message, eventCode, null, data);

        private sealed record LogEntry(
            string EventCode,
            string EventDescription,
            string Message,
            IDictionary<string, object> Context,
            object? Data);

        private void Log(LogLevel level, string message, IEventCode eventCode, Exception? exception, object? data)
        {
            if (!_logger.IsEnabled(level))
                return;

            var contextData = new Dictionary<string, object>();

            if (_options.EnableEnrichers)
                foreach (var enricher in _enrichers)
                    enricher.Enrich(contextData);

            var logEntry = new LogEntry(
                EventCode: eventCode.Code,
                EventDescription: eventCode.Description,
                Message: message,
                Context: contextData,
                Data: data);

            var json = JsonSerializer.Serialize(logEntry, _jsonOptions);

            using (_logger.BeginScope(contextData))
                _logger.Log(
                    level,
                    default,
                    logEntry,
                    exception,
                    (_, __) => json);
        }
    }
}
