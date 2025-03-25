using System.Linq.Dynamic.Core;

using CourseLibrary.API.Services;

namespace CourseLibrary.API.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(
            this IQueryable<T> source, string orderBy, Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (mappingDictionary == null)
            {
                throw new ArgumentNullException(nameof(mappingDictionary));
            }

            if (orderBy.IsEmpty())
            {
                return source;
            }

            /* orderBy looks like: "OrderBy=age, mainCategory desc" */
            string orderByString = string.Empty;
            string[] orderByAfterSplit = orderBy.Split(",");

            foreach (string orderByClause in orderByAfterSplit)
            {
                string trimmedOrderByClause = orderByClause.Trim();

                // if sort option ends with " desc", we order descending, otherwise ascending
                bool orderDescending = trimmedOrderByClause.EndsWith(" desc");

                // remove " asc" or " desc" from the orderBy clause to get property name
                // to look for mapping dictionary
                int indexOfFirstSpace = trimmedOrderByClause.IndexOf(" ");
                string propertyName = indexOfFirstSpace == -1
                    ? trimmedOrderByClause
                    : trimmedOrderByClause.Remove(indexOfFirstSpace);

                // find matching property
                if (!mappingDictionary.ContainsKey(propertyName))
                {
                    throw new ArgumentException($"Key mapping for {propertyName} is missing");
                }

                var propertyMappingValue = mappingDictionary[propertyName];
                if (propertyMappingValue == null)
                {
                    throw new ArgumentException(nameof(propertyMappingValue));
                }

                if (propertyMappingValue.Revert)
                {
                    orderDescending = !orderDescending;
                }

                foreach (var destinationProperty in propertyMappingValue.DestinationProperties)
                {
                    orderByString = orderByString + (orderByString.IsEmpty() ? string.Empty : ", ")
                        + destinationProperty
                        + (orderDescending ? " descending" : " ascending");
                }
            }

            // OrderBy accepts a string {propertyName descending/ascending} "FirstName ascending, LastName descending"
            return source.OrderBy(orderByString);
        }
    }
}
