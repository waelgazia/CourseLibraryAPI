using CourseLibrary.API.Helpers;

namespace CourseLibrary.API.ResourceParameters
{
    public class AuthorsResourceParameters : EntityPagination
    {
        public string? MainCategory { get; set; } = string.Empty;
        public string? SearchQuery { get; set; } = string.Empty;
        public string? OrderBy { get; set; } = "Name";
    }
}
