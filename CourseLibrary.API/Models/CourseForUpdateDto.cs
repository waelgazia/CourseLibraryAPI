using System.ComponentModel.DataAnnotations;

using CourseLibrary.API.Entities;
using CourseLibrary.API.Profiles;

namespace CourseLibrary.API.Models;

public class CourseForUpdateDto : CourseForManipulationDto, IMapFrom<Course>
{
    [Required(ErrorMessage = "You should fill out a description.")]
    public override string Description
    {
        get { return base.Description;  }
        set { base.Description = value; }
    }
}

