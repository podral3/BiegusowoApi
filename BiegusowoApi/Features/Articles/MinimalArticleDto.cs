namespace BiegusowoApi.Features.Articles;

public record MinimalArticleDto(
    string Title,
    string AuthorName,
    string CoverUrl,
    int ReadingTimeMinutes,
    DateTimeOffset PublishedAt,
    string Slug);