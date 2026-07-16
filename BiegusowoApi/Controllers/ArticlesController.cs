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

    public async Task<ActionResult<PaginatedList<MinimalArticleDto>>> GetArticles([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        throw new NotImplementedException();
    }

    public async Task<ActionResult<ArticleDto>> GetArticleBySlug([FromQuery] string slug)
    {
        throw new NotImplementedException();
    }
}
