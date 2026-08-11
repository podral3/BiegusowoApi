using BiegusowoApi.Data;
using BiegusowoApi.Features.Blobs.Dtos;
using BiegusowoApi.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiegusowoApi.Features.Articles;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController(ApplicationDbContext dbContext) : ControllerBase
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    [HttpGet]
    [EndpointDescription("Get a paginated list of minimal article data to display.")]
    [ProducesResponseType(typeof(CursorPaginatedList<MinimalArticleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<CursorPaginatedList<MinimalArticleDto>>> GetArticles(
        [FromQuery] DateTimeOffset? beforePublishedAt,
        [FromQuery] Guid? beforeArticleId,
        [FromQuery] int pageSize = 10)
    {
        var query = _dbContext.Articles.AsQueryable();

        if (beforePublishedAt.HasValue && beforeArticleId.HasValue)
        {
            query = query.Where(a =>
                a.PublishedAt < beforePublishedAt.Value ||
                (a.PublishedAt == beforePublishedAt.Value && a.Id.CompareTo(beforeArticleId.Value) < 0));
        }

        var articles = await query
            .OrderByDescending(a => a.PublishedAt)
            .ThenByDescending(a => a.Id)
            .Take(pageSize + 1)
            .Select(a => new MinimalArticleDto(a.Title, "John Doe", a.CoverURL, a.ReadingTimeMinutes, a.PublishedAt, a.Slug))
            .ToListAsync();

        bool hasNextPage = articles.Count > pageSize;
        if (hasNextPage) articles.RemoveAt(articles.Count - 1);

        return Ok(new CursorPaginatedList<MinimalArticleDto>(articles, hasNextPage));
    }

    [HttpGet("{id:guid}")]
    [EndpointDescription("Get a specific article by its ID.")]
    [ProducesResponseType(typeof(ArticleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ArticleDto>> GetArticle([FromRoute] Guid id)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpPost("{id:guid}/images/presigned")]
    [EndpointDescription("Generate presigned URLs for uploading images associated with a specific article.")]
    [ProducesResponseType(typeof(PresignedUploadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PresignedUploadResponse>> UploadPresigned(
        [FromRoute] Guid id,
        [FromBody] PresignedUploadRequest request)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpPost("{id:guid}/images")]
    [EndpointDescription("Confirm the upload of images associated with a specific article.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmUpload(
        [FromRoute] Guid id,
        [FromBody] List<Guid> keys)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpDelete("{id:guid}/images/")]
    [EndpointDescription("Delete images associated with a specific article.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteImage(
        [FromRoute] Guid id,
        [FromBody] List<Guid> imageIds)
    {
        throw new NotImplementedException();
    }
}
