using StructLog.Interfaces;

namespace StructLog.Models
{
    public record EventCode(string Code, string Description) : IEventCode;
}
