using StructLog.Interfaces;

namespace StructLog.Enrichers
{
    public class MachineNameEnricher : ILogEnricher
    {
        public void Enrich(IDictionary<string, object> context)
        {
            context["MachineName"] = Environment.MachineName;
        }
    }
}
