namespace BiegusowoApi.Data.Models
{
    public class Article
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Excerpt { get; set; }
        public string CoverURL { get; set; }
        public string BodyHTML { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string OgImageUrl { get; set; }
        public string Status { get; set; }
        public int ReadingTimeMinutes { get; set; }
        public DateTimeOffset PublishedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
    }
}