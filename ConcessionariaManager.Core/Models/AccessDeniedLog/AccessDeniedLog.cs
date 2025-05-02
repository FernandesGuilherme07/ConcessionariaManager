namespace ConcessionariaManager.Core.Models.AccessDeniedLog
{
    public class AccessDeniedLog
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required string UserName { get; set; }
        public required string AttemptedUrl { get; set; }
        public string? Location { get; set; }
        public DateTime AccessDateTime { get; set; }
    }

}
