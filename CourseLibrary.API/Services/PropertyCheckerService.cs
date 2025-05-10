using System.Reflection;

namespace CourseLibrary.API.Services
{
    public class PropertyCheckerService : IPropertyCheckerService
    {
        public bool TypeHasProperties<T>(string? fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            string[] fieldsAfterSplit = fields.Split(',');
            foreach (var field in fieldsAfterSplit)
            {
                string propertyName = field.Trim();

                PropertyInfo? propertyInfo = typeof(T)
                    .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo == null)
                    return false;
            }

            return true;
        }
    }
}
