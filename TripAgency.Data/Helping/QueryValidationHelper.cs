namespace TripAgency.Data.Helping
{
    public static class QueryValidationHelper
    {
        public static void ValidatePaginationParameters(ref int pageNumber, ref int pageSize)
        {
            pageNumber = pageNumber <= 0 ? PaginationSettings.DefaultPageNumber : pageNumber;
            pageSize = pageSize <= 0 ? PaginationSettings.DefaultPageSize : pageSize;
            pageSize = Math.Min(pageSize, PaginationSettings.MaxPageSize);
        }

        public static void ValidateFilterParameters(Dictionary<string, string> filters, string[] allowedFilterProperties)
        {
            if (filters == null) return;

            foreach (var filter in filters)
            {
                if (!allowedFilterProperties.Contains(filter.Key))
                {
                    throw new ArgumentException($"Filtering by '{filter.Key}' is not allowed");
                }

                if (string.IsNullOrEmpty(filter.Value) || filter.Value.Length > FilterSettings.MaxFilterValueLength)
                {
                    throw new ArgumentException($"Invalid filter value for '{filter.Key}'");
                }
            }
        }

        public static void ValidateSortParameters(ref string sortColumn, ref string sortDirection, string[] allowedSortProperties)
        {
            if (string.IsNullOrEmpty(sortColumn) || !allowedSortProperties.Contains(sortColumn))
            {
                sortColumn = "Id";
            }

            if (string.IsNullOrEmpty(sortDirection) ||
                (!sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase) &&
                 !sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)))
            {
                sortDirection = "asc";
            }
        }

        public static string SanitizeSearchTerm(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return string.Empty;

            var sanitized = searchTerm.Trim();
            return sanitized.Length > SearchSettings.MaxSearchTermLength
                ? sanitized.Substring(0, SearchSettings.MaxSearchTermLength)
                : sanitized;
        }

        public static bool PropertyExists<T>(string propertyName)
        {
            return typeof(T).GetProperty(propertyName) != null;
        }
    }
}
