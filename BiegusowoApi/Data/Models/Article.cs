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
        public string cover_url { get; set; }
        public string body_html { get; set; }
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