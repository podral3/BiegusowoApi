namespace BiegusowoApi.Domain.Dtos.Article;

public record MinimalArticleDto(
    string Title,
    string AuthorName,
    string CoverUrl,
    int ReadingTimeMinutes,
    DateTimeOffset PublishedAt,
    string Slug);