using System.Reflection;

using AutoMapper;

namespace CourseLibrary.API.Profiles
{
    public interface IMapFrom<TSource>
    {
        void Mapping(Profile profile) => profile.CreateMap(typeof(TSource), GetType()).ReverseMap();
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            Assembly[] assemblies = [ Assembly.GetExecutingAssembly() ];

            foreach (Assembly assembly in assemblies)
            {
                ApplyMappingFromAssembly(assembly);
            }
        }

        public void ApplyMappingFromAssembly(Assembly assembly)
        {
            // retrieve types that implemented the IMapFrom<> interface
            List<Type> types = assembly.GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                .ToList();

            foreach (Type type in types)
            {
                // invoke the IMapFrom<>.Mapping method
                object? instance = Activator.CreateInstance(type);

                MethodInfo? mappingMethod = type.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>))
                    ?.GetMethod(nameof(IMapFrom<object>.Mapping));

                mappingMethod?.Invoke(instance, [this]);
            }
        }
    }
}
