using CourseLibrary.API.Entities;
using CourseLibrary.API.Profiles;

namespace CourseLibrary.API.Models
{
    public class AuthorForCreationDto : IMapFrom<Author>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTimeOffset DateOfBirth { get; set; }
        public string MainCategory { get; set; } = string.Empty;
        public ICollection<CourseForCreationDto> Courses { get; set; } = new List<CourseForCreationDto>();
    }
}
