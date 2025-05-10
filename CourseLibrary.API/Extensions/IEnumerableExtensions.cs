using System.Dynamic;
using System.Reflection;

namespace CourseLibrary.API.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<ExpandoObject> 
            ShapeData<TSource>(this IEnumerable<TSource> source, string fields)
        {
            if (source == null)
            {
                ArgumentNullException.ThrowIfNull(nameof(source));
            }

            List<ExpandoObject> expandoObjectList = new List<ExpandoObject>();
            
            // Reflection is expensive, so rather than doing it for each object in the list,
            // we do it once and reuse the results.
            List<PropertyInfo> propertyInfoList = new List<PropertyInfo>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                // if no specific fields are requested, add all public properties
                // to the expandoOjbect.
                var propertyInfos = typeof(TSource)
                    .GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                propertyInfoList.AddRange(propertyInfos);
            }
            else
            {
                // fields are usually separated by comma i.e., firstName,lastName, ... etc
                string[] fieldsAfterSplit = fields.Split(',');
                foreach (var field in fieldsAfterSplit)
                {
                    string propertyName = field.Trim();

                    var propertyInfo = typeof(TSource)
                        .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        throw new Exception($"Proeprty {propertyName} was not found on {typeof(TSource)}");
                    }

                    propertyInfoList.Add(propertyInfo);
                }
            }

            // run through the source objects, and create an expandoObject that will hold the selected properties
            foreach (TSource sourceObject in source)
            {
                ExpandoObject dataShapedObject = new ExpandoObject();

                foreach (PropertyInfo propertyInfo in propertyInfoList)
                {
                    var propertyValue = propertyInfo.GetValue(sourceObject);

                    ((IDictionary<string, object?>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
                }

                expandoObjectList.Add(dataShapedObject);
            }

            return expandoObjectList;
        }
    }
}
