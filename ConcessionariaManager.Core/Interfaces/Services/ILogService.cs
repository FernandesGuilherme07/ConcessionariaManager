namespace ConcessionariaManager.Core.Interfaces.Services
{
    public interface ILogService
    {
        Task LogCreateAsync(string user, string screen, Dictionary<string, string> newValues);
        Task LogEditAsync(string user, string screen, Dictionary<string, (string OldValue, string NewValue)> modifiedValues);
    }
}
