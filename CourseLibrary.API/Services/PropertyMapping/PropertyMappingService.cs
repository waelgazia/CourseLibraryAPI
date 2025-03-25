using CourseLibrary.API.Models;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Extensions;

namespace CourseLibrary.API.Services
{
    /*
     PropertyMappingService : IPropertyMappingService
        IList<PropertyMapping> propertyMappings
            PropertyMapping<TSource, TDestination> : IPropertyMapping
                Dictionary<string, PropertyMappingValue>
                    PropertyMappingValue
                        IEnumerable<string> DestinationProperties
                        Revert

        GetPropertyMapping<TSource, TDestination>() 
    */

    public class PropertyMappingService : IPropertyMappingService
    {
        private readonly IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        private readonly Dictionary<string, PropertyMappingValue> _authorPropertyMapping
           = new(StringComparer.OrdinalIgnoreCase)
           {
               { nameof(AuthorDto.Id), new ([nameof(Author.Id)]) },
               { nameof(AuthorDto.MainCategory), new ([nameof(Author.MainCategory)]) },
               { nameof(AuthorDto.Age), new ([nameof(Author.DateOfBirth)], true) },
               { nameof(AuthorDto.Name), new ([nameof(Author.FirstName), nameof(Author.LastName)]) }
           };

        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<AuthorDto, Author>(_authorPropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First().MappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance " +
                $"for <{typeof(TSource)}, {typeof(TDestination)}>");
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();
            if (fields.IsEmpty())
            {
                return true;
            }

            string[] fieldsAfterSplit = fields.Split(",");
            foreach (string field in fieldsAfterSplit)
            {
                string trimmedField = field.Trim();

                int indexOfFirstSpace = field.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 
                    ? trimmedField 
                    : trimmedField.Remove(indexOfFirstSpace);

                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
