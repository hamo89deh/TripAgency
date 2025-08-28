namespace TripAgency.Data.Helping
{
    public class QueryParameters
    {
        private int _pageNumber = PaginationSettings.DefaultPageNumber;
        private int _pageSize = PaginationSettings.DefaultPageSize;
        private string _sortColumn = "Id";
        private string _sortDirection = "asc";

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value <= 0 ? PaginationSettings.DefaultPageNumber : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value <= 0 ? PaginationSettings.DefaultPageSize :
                Math.Min(value, PaginationSettings.MaxPageSize);
        }

        public string SortColumn
        {
            get => _sortColumn;
            set => _sortColumn = string.IsNullOrEmpty(value) ? "Id" : value;
        }

        public string SortDirection
        {
            get => _sortDirection;
            set => _sortDirection = string.IsNullOrEmpty(value) ||
                                   (!value.Equals("asc", StringComparison.OrdinalIgnoreCase) &&
                                    !value.Equals("desc", StringComparison.OrdinalIgnoreCase))
                ? "asc" : value.ToLower();
        }

        public string SearchTerm { get; set; }
        public Dictionary<string, string> Filters { get; set; }
    }
}
