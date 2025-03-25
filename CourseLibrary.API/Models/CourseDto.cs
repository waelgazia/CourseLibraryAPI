using CourseLibrary.API.Entities;
using CourseLibrary.API.Profiles;

namespace CourseLibrary.API.Models;
public class CourseDto : IMapFrom<Course>
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
}
