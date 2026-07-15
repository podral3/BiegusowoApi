namespace BiegusowoApi.Data.Models
{
    public class ListingPhoto
    {
        public Guid Id { get; set; }
        
        public Guid ListingId { get; set; }
        public Listing Listing { get; set; } = null!;

        public string Url { get; set; }
        public int SortOrder { get; set; }
        public bool IsCover { get; set; }
        public string Bucket { get; set; }
        public string StorageProvider { get; set; } 
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    }
}
