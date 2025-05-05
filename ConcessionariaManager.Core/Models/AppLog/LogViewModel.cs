namespace ConcessionariaManager.Core.Models.AppLog
{
    namespace ConcessionariaManager.Core.Models.AppLog
    {
        public class LogViewModel
        {
            public List<Log> Logs { get; set; } = new();
            public string? User { get; set; }
            public string? Screen { get; set; }
            public DateTime? Date { get; set; }
            public string? Action { get; set; }
            public int CurrentPage { get; set; }
            public int TotalPages { get; set; }
        }
    }

}
