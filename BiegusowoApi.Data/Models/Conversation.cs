namespace BiegusowoApi.Data.Models
{
    public class Conversation
    {
        public Guid Id { get; set; }
        
        public Guid ListingId { get; set; }
        public Listing Listing { get; set; } = null!;

        public Guid BuyerId { get; set; }
        public User Buyer { get; set; } = null!;

        public Guid SellerId { get; set; }
        public User Seller { get; set; } = null!; 
        public DateTimeOffset CreatedAt { get; set; }
    }
}
