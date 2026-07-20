namespace BiegusowoApi.Data.Models
{
    public class ListingImage
    {
        public Guid Id { get; set; }
        
        public Guid ListingId { get; set; }
        public Listing Listing { get; set; } = null!;

        public string FileName { get; set; }
        public int SortOrder { get; set; }
        public string Bucket { get; set; }
        public int FileSizeBytes { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    }
}
