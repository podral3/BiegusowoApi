using BiegusowoApi.Data;
using BiegusowoApi.Domain.Dtos.Article;
using BiegusowoApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiegusowoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController(ApplicationDbContext dbContext) : ControllerBase
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    [HttpGet]
    public async Task<ActionResult<PaginatedList<MinimalArticleDto>>> GetArticles([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ArticleDto>> GetArticle([FromRoute] string id)
    {
        throw new NotImplementedException();
    }
}
