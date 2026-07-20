using BiegusowoApi.Data;
using BiegusowoApi.Domain.Dtos.Article;
using BiegusowoApi.Domain.Image;
using BiegusowoApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiegusowoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController(ApplicationDbContext dbContext) : ControllerBase
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    [HttpGet]
    [EndpointDescription("Get a paginated list of miminal article data to display.")]
    [ProducesResponseType(typeof(PaginatedList<MinimalArticleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<MinimalArticleDto>>> GetArticles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        throw new NotImplementedException();
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
