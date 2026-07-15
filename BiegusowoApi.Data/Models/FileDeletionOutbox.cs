namespace BiegusowoApi.Data.Models
{
    public class FileDeletionOutbox
    {
        public Guid Id { get; set; }
        public string Bucket { get; set; }
        public string StorageKey { get; set; }
        public string Status { get; set; }
        public int RetryCount { get; set; }
        public string LastError { get; set; }
        public string StorageProvider { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? LastRetryAt { get; set; }
    }
}
