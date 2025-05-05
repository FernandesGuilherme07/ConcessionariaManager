namespace ConcessionariaManager.Core.Models.AppLog
{
    public class Log
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string User { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Screen { get; set; } = string.Empty;

        public ICollection<LogDetail>? LogDetails { get; set; }
    }

}
