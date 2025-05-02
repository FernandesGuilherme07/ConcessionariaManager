namespace ConcessionariaManager.Core.Models.AccessDeniedLog
{
    public class AccessDeniedLogViewModel
    {
        public List<AccessDeniedLog> Logs { get; set; }

        public string? UserName { get; set; }
        public DateTime? Date { get; set; }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

}
