using AutoMapper;

using CourseLibrary.API.Helpers;
using CourseLibrary.API.Profiles;
using CourseLibrary.API.Entities;

namespace CourseLibrary.API.Models;

public class AuthorDto : IMapFrom<Author>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string MainCategory { get; set; } = string.Empty;

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Author, AuthorDto>()
            .ForMember(dest => dest.Name, opt =>
                opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Age, opt =>
                opt.MapFrom(src => src.DateOfBirth.GetCurrentAge()));
    }
}
