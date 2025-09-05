namespace TripAgency.Data.Helping
{
    public class PaginatedResult<T>
    {


        public PaginatedResult(List<T> data)
        {
            Data = data;
        }

        internal PaginatedResult(bool succeeded, List<T> data = default, List<string> messages = null, int totalCount = 0,
            int page = 1, int pageSize = 10)
        {
            Data = data;
            CurrentPage = page;
            Succeeded = succeeded;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            TotalCount = totalCount;
        }

        public static PaginatedResult<T> Success(List<T> data, int totalCount, int page, int pageSize)
        {
            return new(true, data, null, totalCount, page, pageSize);
        }
        public List<T> Data { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }

        public object Meta { get; set; }

        public int PageSize { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;

        public bool HasNextPage => CurrentPage < TotalPages;

        public List<string> Messages { get; set; } = new();

        public bool Succeeded { get; set; }
    }
}
