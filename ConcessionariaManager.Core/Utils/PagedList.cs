using ConcessionariaManager.Core.Interfaces;
using System.Collections;

namespace ConcessionariaManager.Core.Utils
{
    public class PagedList<T> : IPagedList<T>
    {
        private readonly List<T> _items;

        public PagedList(IEnumerable<T> items, int pageNumber, int pageSize, int totalItemCount)
        {
            _items = items.ToList();
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalItemCount = totalItemCount;
            PageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
        }

        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalItemCount { get; }
        public int PageCount { get; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < PageCount;

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var count = await Task.Run(() => source.Count(), cancellationToken);
            var items = await Task.Run(() => source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), cancellationToken);

            return new PagedList<T>(items, pageNumber, pageSize, count);
        }
        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

}
