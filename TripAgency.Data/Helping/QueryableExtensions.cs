using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace TripAgency.Data.Helping
{
    public static class QueryableExtensions
    {
        public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T>(this IQueryable<T> query, int pageNumber, int pageSize)
            where T : class
        {
            if (query == null)
            {
                throw new Exception("Empty");
            }

            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            int count = await query.AsNoTracking().CountAsync();
            if (count == 0)
                return PaginatedResult<T>.Success(new List<T>(), count, pageNumber, pageSize);
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return PaginatedResult<T>.Success(items, count, pageNumber, pageSize);
        }
        public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, Dictionary<string, string> filters, string[] allowedFilterProperties = null)
                where T : class
        {
            if (filters == null || !filters.Any())
            {
                return query;
            }
            // استخدام الخصائص المسموح بها افتراضيًا إذا لم يتم توفيرها
            allowedFilterProperties ??= typeof(T).GetProperties()
                .Where(p => p.PropertyType == typeof(string) || p.PropertyType.IsValueType)
                .Select(p => p.Name)
                .ToArray();

            QueryValidationHelper.ValidateFilterParameters(filters, allowedFilterProperties);
            foreach (var filter in filters)
            {
                if (!QueryValidationHelper.PropertyExists<T>(filter.Key)) continue;
                var propertyName = filter.Key;
                var filterValue = filter.Value;
                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, propertyName);
                var constant = Expression.Constant(filterValue);
                var equals = Expression.Equal(property, constant);
                var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);
                query = query.Where(lambda);
            }
            return query;
        }
        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string sortColumn, string sortDirection, string[] allowedSortProperties = null)
             where T : class
        {
            if (string.IsNullOrEmpty(sortColumn))
            {
                return query;
            }

            // استخدام الخصائص المسموح بها افتراضيًا إذا لم يتم توفيرها
            allowedSortProperties ??= FilterSettings.DefaultAllowedSortProperties;

            QueryValidationHelper.ValidateSortParameters(ref sortColumn, ref sortDirection, allowedSortProperties);

            if (!QueryValidationHelper.PropertyExists<T>(sortColumn)) return query;


            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, sortColumn);
            var lambda = Expression.Lambda(property, parameter);
            var methodName = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase) ? "OrderByDescending" : "OrderBy";
            var methodCall = Expression.Call(
            typeof(Queryable),
            methodName,
            new[] { typeof(T), property.Type },
            query.Expression,
            Expression.Quote(lambda)
            );

            return query.Provider.CreateQuery<T>(methodCall);
        }

        public static IQueryable<T> ApplySearch<T>(this IQueryable<T> query, string searchTerm, string[] searchableProperties)
             where T : class
        {
            var sanitizedSearchTerm = QueryValidationHelper.SanitizeSearchTerm(searchTerm);
            if (string.IsNullOrEmpty(sanitizedSearchTerm)) return query;

            // استخدام الخصائص المسموح بها افتراضيًا إذا لم يتم توفيرها
            searchableProperties ??= typeof(T).GetProperties()
                .Where(p => p.PropertyType == typeof(string))
                .Select(p => p.Name)
                .ToArray();

            if (searchableProperties == null || !searchableProperties.Any()) return query;


            var expression = PredicateBuilder.False<T>();
            foreach (var propertyName in searchableProperties)
            {
                if (!QueryValidationHelper.PropertyExists<T>(propertyName)) continue;
                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, propertyName);
                var ToStringCall = Expression.Call(property, "ToString", null, null);
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var searchExpression = Expression.Call(ToStringCall, containsMethod, Expression.Constant(searchTerm));
                var lambda = Expression.Lambda<Func<T, bool>>(searchExpression, parameter);

                expression = expression.Or(lambda);
            }
            return query.Where(expression);
        }
    }

    //public static class QueryableExtensions
    //{
    //    public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    //        where T : class
    //    {
    //        if (query == null)
    //        {
    //            throw new Exception("Empty");
    //        }

    //        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
    //        pageSize = pageSize <= 0 ? 10 : pageSize;
    //        int count = await query.AsNoTracking().CountAsync();
    //        if (count == 0)
    //            return PaginatedResult<T>.Success(new List<T>(), count, pageNumber, pageSize);
    //        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    //        return PaginatedResult<T>.Success(items, count, pageNumber, pageSize);
    //    }
    //    public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, Dictionary<string, string> filters, string[] allowedFilterProperties = null)
    //            where T : class
    //    {
    //        if (filters == null || !filters.Any())
    //        {
    //            return query;
    //        }
    //        // استخدام الخصائص المسموح بها افتراضيًا إذا لم يتم توفيرها
    //        allowedFilterProperties ??= typeof(T).GetProperties()
    //            .Where(p => p.PropertyType == typeof(string) || p.PropertyType.IsValueType)
    //            .Select(p => p.Name)
    //            .ToArray();

    //        QueryValidationHelper.ValidateFilterParameters(filters, allowedFilterProperties);
    //        foreach (var filter in filters)
    //        {
    //            if (!QueryValidationHelper.PropertyExists<T>(filter.Key)) continue;
    //            var propertyName = filter.Key;
    //            var filterValue = filter.Value;
    //            var parameter = Expression.Parameter(typeof(T), "x");
    //            var property = Expression.Property(parameter, propertyName);
    //            var constant = Expression.Constant(filterValue);
    //            var equals = Expression.Equal(property, constant);
    //            var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);
    //            query = query.Where(lambda);
    //        }
    //        return query;
    //    }
    //    public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string sortColumn, string sortDirection, string[] allowedSortProperties = null)
    //         where T : class
    //    {
    //        if (string.IsNullOrEmpty(sortColumn))
    //        {
    //            return query;
    //        }

    //        // استخدام الخصائص المسموح بها افتراضيًا إذا لم يتم توفيرها
    //        allowedSortProperties ??= FilterSettings.DefaultAllowedSortProperties;

    //        QueryValidationHelper.ValidateSortParameters(ref sortColumn, ref sortDirection, allowedSortProperties);

    //        if (!QueryValidationHelper.PropertyExists<T>(sortColumn)) return query;


    //        var parameter = Expression.Parameter(typeof(T), "x");
    //        var property = Expression.Property(parameter, sortColumn);
    //        var lambda = Expression.Lambda(property, parameter);
    //        var methodName = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase) ? "OrderByDescending" : "OrderBy";
    //        var methodCall = Expression.Call(
    //        typeof(Queryable),
    //        methodName,
    //        new[] { typeof(T), property.Type },
    //        query.Expression,
    //        Expression.Quote(lambda)
    //        );

    //        return query.Provider.CreateQuery<T>(methodCall);
    //    }

    //    public static IQueryable<T> ApplySearch<T>(this IQueryable<T> query, string searchTerm, string[] searchableProperties)
    //         where T : class
    //    {
    //        var sanitizedSearchTerm = QueryValidationHelper.SanitizeSearchTerm(searchTerm);
    //        if (string.IsNullOrEmpty(sanitizedSearchTerm)) return query;

    //        // استخدام الخصائص المسموح بها افتراضيًا إذا لم يتم توفيرها
    //        searchableProperties ??= typeof(T).GetProperties()
    //            .Where(p => p.PropertyType == typeof(string))
    //            .Select(p => p.Name)
    //            .ToArray();

    //        if (searchableProperties == null || !searchableProperties.Any()) return query;


    //        var expression = PredicateBuilder.False<T>();
    //        foreach (var propertyName in searchableProperties)
    //        {
    //            if (!QueryValidationHelper.PropertyExists<T>(propertyName)) continue;
    //            var parameter = Expression.Parameter(typeof(T), "x");
    //            var property = Expression.Property(parameter, propertyName);
    //            var ToStringCall = Expression.Call(property, "ToString", null, null);
    //            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
    //            var searchExpression = Expression.Call(ToStringCall, containsMethod, Expression.Constant(searchTerm));
    //            var lambda = Expression.Lambda<Func<T, bool>>(searchExpression, parameter);

    //            expression = expression.Or(lambda);
    //        }
    //        return query.Where(expression);
    //    }
    //}
}
