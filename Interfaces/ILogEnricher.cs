namespace StructLog.Interfaces
{
    public interface ILogEnricher
    {
        public void Enrich(IDictionary<string, object> context);
    }
}
