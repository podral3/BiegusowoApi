namespace BiegusowoApi.Data.Models
{
    public class Article
    {
        public int Id { get; set; }
        
        public int AuthorId { get; set; }
        public User Author { get; set; } = null!;
        
        public string Title { get; set; }
        public string slug { get; set; }
        public string Excerpt { get; set; }
        public string CoverURL { get; set; }
        public string BodyHTML { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string OgImageUrl { get; set; }
        public string Status { get; set; }
        public int ReadingTimeMinutes { get; set; }
        public DateTimeOffset PublishedAt { get; set; }
        public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; }
    }
}