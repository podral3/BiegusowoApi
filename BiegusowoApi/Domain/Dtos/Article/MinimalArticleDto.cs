namespace BiegusowoApi.Domain.Dtos.Article;

public record MinimalArticleDto(
    string Title,
    string AuthorName,
    string ConverUrl,
    int ReadingTimeMinuts,
    DateTimeOffset PublishedAt,
    string Slug);