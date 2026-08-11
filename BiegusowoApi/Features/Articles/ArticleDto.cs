namespace BiegusowoApi.Features.Articles;

public record ArticleDto(
    string AuthorName,
    string Title,
    string BodyHtml,
    string CoverUrl,
    DateTimeOffset CreatedAt,
    string Excerpt,
    string MetaTitle,
    string MetaDescription,
    string OgImageUrl,
    int ReadingTimeMinutes,
    string Slug,
    string SourcePath,
    DateTimeOffset? UpdatedAt);