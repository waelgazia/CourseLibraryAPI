using CourseLibrary.API.Entities;
using CourseLibrary.API.Profiles;

namespace CourseLibrary.API.Models;

public class CourseForCreationDto : CourseForManipulationDto, IMapFrom<Course>
{

}

