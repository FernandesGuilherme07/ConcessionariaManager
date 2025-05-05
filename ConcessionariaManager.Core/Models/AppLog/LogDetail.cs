namespace ConcessionariaManager.Core.Models.AppLog
{
    public class LogDetail
    {
        public long Id { get; set; }
        public long LogId { get; set; }
        public string Field { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        public Log Log { get; set; }
    }

}
