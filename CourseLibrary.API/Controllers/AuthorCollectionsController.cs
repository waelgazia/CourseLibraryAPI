using Microsoft.AspNetCore.Mvc;

using AutoMapper;

using CourseLibrary.API.Models;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Services;
using CourseLibrary.API.Entities;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorCollectionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICourseLibraryRepository _courseLibraryRepository;

        public AuthorCollectionsController(IMapper mapper, ICourseLibraryRepository courseLibraryRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _courseLibraryRepository = courseLibraryRepository
                ?? throw new ArgumentNullException(nameof(courseLibraryRepository));
        }

        [HttpGet("({authorIds})", Name = nameof(GetAuthorCollection))]
        public async Task<ActionResult<IEnumerable<AuthorDto>>>
            GetAuthorCollection(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))]
            [FromRoute] IEnumerable<Guid> authorIds)
        {
            var authorEntities = await _courseLibraryRepository.GetAuthorsAsync(authorIds);

            if (authorIds.Count() != authorEntities.Count())
            {
                return NotFound();
            }

            var authorToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
            return Ok(authorToReturn);
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<AuthorDto>>>
            CreateAuthorCollection(IEnumerable<AuthorForCreationDto> authorCollection)
        {
            var authorEntities = _mapper.Map<IEnumerable<Author>>(authorCollection);

            foreach (var author in authorEntities)
            {
                _courseLibraryRepository.AddAuthor(author);
            }
            await _courseLibraryRepository.SaveAsync();

            var authorCollectionToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
            string authodCollectionIds = string.Join(",", authorCollectionToReturn.Select(a => a.Id));

            return CreatedAtRoute(nameof(GetAuthorCollection),
                new { authorIds = authodCollectionIds },
                authorCollectionToReturn);
        }

        [HttpOptions]
        public IActionResult GetAuthorCollectionOptions()
        {
            Response.Headers.Append("Allow", "GET,POST,OPTIONS");
            return Ok();
        }
    }
}
