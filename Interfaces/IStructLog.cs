namespace StructLog.Interfaces
{
    public interface IStructLog<T> where T : class
    {
        public void Info(string message, IEventCode eventCode, object? data = null);
        public void Warning(string message, IEventCode eventCode, object? data = null);
        public void Error(string message, IEventCode eventCode, Exception? ex = null, object? data = null);

        public IDisposable BeginScope(IDictionary<string, object> context);
    }
}
