namespace ConcessionariaManager.Core.Interfaces
{
    public interface IPagedList<T> : IEnumerable<T>
    {
        int PageNumber { get; }
        int PageSize { get; }
        int TotalItemCount { get; }
        int PageCount { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
    }

}
