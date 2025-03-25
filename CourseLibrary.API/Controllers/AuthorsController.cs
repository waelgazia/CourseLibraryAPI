using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

using AutoMapper;

using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Entities;

namespace CourseLibrary.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly IPropertyMappingService _propertyMappingService;
    private readonly ICourseLibraryRepository _courseLibraryRepository;
    private readonly IMapper _mapper;

    public AuthorsController(
        IPropertyMappingService propertyMappingService,
        ICourseLibraryRepository courseLibraryRepository,
        IMapper mapper)
    {
        _courseLibraryRepository = courseLibraryRepository ??
            throw new ArgumentNullException(nameof(courseLibraryRepository));
        _mapper = mapper ??
            throw new ArgumentNullException(nameof(mapper));
        _propertyMappingService = propertyMappingService ?? 
            throw new ArgumentNullException(nameof(propertyMappingService));
    }

    [HttpGet]
    [HttpHead(Name = nameof(GetAuthors))]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> 
        GetAuthors([FromQuery] AuthorsResourceParameters authorsResourceParameters)
    {
        if (!_propertyMappingService.ValidMappingExistsFor<AuthorDto, Author>(authorsResourceParameters.OrderBy))
        {
            return BadRequest();
        }

        var authorsFromRepo = await _courseLibraryRepository
            .GetAuthorsAsync(authorsResourceParameters);

        string? previousPageLink = authorsFromRepo.HasPrevious 
            ? CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.PREVIOUS_PAGE) 
            : null;

        string? nextPageLink = authorsFromRepo.HasNext
            ? CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.NEXT_PAGE)
            : null;

        var paginationMetadata = new
        {
            totalCount = authorsFromRepo.TotalCount,
            pageSize = authorsFromRepo.PageSize,
            currentPage = authorsFromRepo.CurrentPage,
            totalPages = authorsFromRepo.TotalPages,
            previousPageLink = previousPageLink,
            nextPageLink = nextPageLink
        };

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo));
    }

    [HttpGet("{authorId}", Name = "GetAuthor")]
    public async Task<ActionResult<AuthorDto>> GetAuthor(Guid authorId)
    {
        var authorFromRepo = await _courseLibraryRepository.GetAuthorAsync(authorId);

        if (authorFromRepo == null)
        {
            return NotFound();
        }

        // return author
        return Ok(_mapper.Map<AuthorDto>(authorFromRepo));
    }

    [HttpPost]
    public async Task<ActionResult<AuthorDto>> CreateAuthor(AuthorForCreationDto author)
    {
        var authorEntity = _mapper.Map<Entities.Author>(author);

        _courseLibraryRepository.AddAuthor(authorEntity);
        await _courseLibraryRepository.SaveAsync();

        var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);

        return CreatedAtRoute("GetAuthor",
            new { authorId = authorToReturn.Id },
            authorToReturn);
    }

    [HttpOptions]
    public IActionResult GetAuthorOptions()
    {
        Response.Headers.Append("Allow", "GET,POST,POST,OPTIONS");
        return Ok();
    }

    private string? CreateAuthorsResourceUri(
        AuthorsResourceParameters authorsResourceParameters, 
        ResourceUriType resourceUriType)
    {
        switch (resourceUriType)
        {
            case ResourceUriType.PREVIOUS_PAGE:
                return Url.Link(nameof(GetAuthors), new
                {
                    orderBy = authorsResourceParameters.OrderBy,
                    pageNumber = authorsResourceParameters.PageNumber - 1,
                    pageSize = authorsResourceParameters.PageSize,
                    searchQuery = authorsResourceParameters.SearchQuery,
                    mainCategory = authorsResourceParameters.MainCategory
                });
            case ResourceUriType.NEXT_PAGE:
                return Url.Link(nameof(GetAuthors), new
                {
                    orderBy = authorsResourceParameters.OrderBy,
                    pageNumber = authorsResourceParameters.PageNumber + 1,
                    pageSize = authorsResourceParameters.PageSize,
                    searchQuery = authorsResourceParameters.SearchQuery,
                    mainCategory = authorsResourceParameters.MainCategory
                });
            default:
                return Url.Link(nameof(GetAuthors), new
                {
                    orderBy = authorsResourceParameters.OrderBy,
                    pageNumber = authorsResourceParameters.PageNumber,
                    pageSize = authorsResourceParameters.PageSize,
                    searchQuery = authorsResourceParameters.SearchQuery,
                    mainCategory = authorsResourceParameters.MainCategory
                });
        }
    }
}
