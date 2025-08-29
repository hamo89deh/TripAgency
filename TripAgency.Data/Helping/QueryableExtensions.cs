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
                var propertyType = typeof(T).GetProperty(propertyName)?.PropertyType;
                if (propertyType == null) continue;

                // تحليل المشغل والقيمة
                var filterParts = ParseFilterExpression(filterValue);
                var @operator = filterParts.Operator;
                var value = filterParts.Value;

                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, propertyName);
                Expression comparison = null;

                if(@operator == "range")
                {
                    comparison = CreateRangeExpression(property, value, propertyType, parameter);
                }
                else
                {
                    // تحويل القيمة إلى النوع المناسب
                    var convertedValue = ConvertValue(value, propertyType);
                    if (convertedValue == null) continue; // إذا فشل التحويل، نتخطى هذا الفلتر
                    var constant = Expression.Constant(convertedValue);
                    comparison = CreateComparisonExpression(property, constant, @operator, propertyType);
                }

                if (comparison != null)
                {
                    var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
                    query = query.Where(lambda);
                }
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
        private static object ConvertValue(string value, Type targetType)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            try
            {
                if (targetType == typeof(string))
                    return value;

                if (targetType == typeof(int) || targetType == typeof(int?))
                    return int.Parse(value);

                if (targetType == typeof(decimal) || targetType == typeof(decimal?))
                    return decimal.Parse(value);

                if (targetType == typeof(double) || targetType == typeof(double?))
                    return double.Parse(value);

                if (targetType == typeof(float) || targetType == typeof(float?))
                    return float.Parse(value);

                if (targetType == typeof(bool) || targetType == typeof(bool?))
                {
                    if (bool.TryParse(value, out bool boolResult))
                        return boolResult;

                    // دعم لـ "true"/"false" أو "1"/"0"
                    if (value == "1") return true;
                    if (value == "0") return false;

                    return null;
                }

                if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
                {
                    if (DateTime.TryParse(value, out DateTime dateResult))
                        return dateResult;

                    return null;
                }

                if (targetType == typeof(Guid) || targetType == typeof(Guid?))
                {
                    if (Guid.TryParse(value, out Guid guidResult))
                        return guidResult;

                    return null;
                }

                if (targetType.IsEnum)
                {
                    if (int.TryParse(value, out int enumIntValue))
                        return Enum.ToObject(targetType, enumIntValue);

                    return Enum.Parse(targetType, value, true);
                }

                // للأنواع الأخرى، حاول استخدام Convert.ChangeType
                return Convert.ChangeType(value, Nullable.GetUnderlyingType(targetType) ?? targetType);
            }
            catch
            {
                // في حالة فشل التحويل، نعود بقيمة null
                return null;
            }
        }
        private static Expression CreateComparisonExpression(MemberExpression property, ConstantExpression constant, string @operator, Type propertyType)
        {
            switch (@operator.ToLower())
            {
                case "eq":
                    return Expression.Equal(property, constant);

                case "gt":
                    return Expression.GreaterThan(property, constant);

                case "lt":
                    return Expression.LessThan(property, constant);

                case "gte":
                    return Expression.GreaterThanOrEqual(property, constant);

                case "lte":
                    return Expression.LessThanOrEqual(property, constant);

                case "contains" when propertyType == typeof(string):
                    return Expression.Call(property, "Contains", null, constant);
           
                case "startswith" when propertyType == typeof(string):
                    return Expression.Call(property, "StartsWith", null, constant);

                case "endswith" when propertyType == typeof(string):
                    return Expression.Call(property, "EndsWith", null, constant);
                default:
                    return Expression.Equal(property, constant);
            }
        }

        private static Expression CreateRangeExpression(MemberExpression property, string rangeValue, Type propertyType, ParameterExpression parameter)
        {
            if (string.IsNullOrEmpty(rangeValue))
                return null;

            var rangeParts = rangeValue.Split(',');
            if (rangeParts.Length != 2)
                return null;

            var minValue = ConvertValue(rangeParts[0], propertyType);
            var maxValue = ConvertValue(rangeParts[1], propertyType);

            if (minValue == null || maxValue == null)
                return null;

            var minConstant = Expression.Constant(minValue, propertyType);
            var maxConstant = Expression.Constant(maxValue, propertyType);

            var greaterThanOrEqual = Expression.GreaterThanOrEqual(property, minConstant);
            var lessThanOrEqual = Expression.LessThanOrEqual(property, maxConstant);

            return Expression.AndAlso(greaterThanOrEqual, lessThanOrEqual);
        }
        private static FilterExpression ParseFilterExpression(string filterValue)
        {
            // الصيغ المتوقعة:
            // "value" => يساوي
            // "gt:value" => أكبر من
            // "lt:value" => أصغر من
            // "gte:value" => أكبر من أو يساوي
            // "lte:value" => أصغر من أو يساوي
            // "range:min,max" => نطاق بين قيمتين

            var parts = filterValue.Split(':');

            if (parts.Length == 1)
            {
                return new FilterExpression { Operator = "eq", Value = parts[0] };
            }

            var supportedOperators = new[] { "eq", "gt", "lt", "gte", "lte", "range", "contains", "startswith", "endswith" };

            if (supportedOperators.Contains(parts[0].ToLower()))
            {
                return new FilterExpression
                {
                    Operator = parts[0].ToLower(),
                    Value = string.Join(":", parts.Skip(1))
                };
            }

            // إذا لم يتطابق مع أي مشغل، نعتبره قيمة عادية
            return new FilterExpression { Operator = "eq", Value = filterValue };
        }

    }
    public class FilterExpression
    {
        public string Operator { get; set; }
        public string Value { get; set; }
    }
}
