using BiegusowoApi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BiegusowoApi.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticlePhoto> ArticlePhotos { get; set; }
        public DbSet<Breed> Breeds { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<FileDeletionOutbox> FileDeletionOutboxes { get; set; }
        public DbSet<Listing> Listings { get; set; }
        public DbSet<ListingPhoto> ListingPhotos { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Species> Species { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Voivodeship> Voivodeships { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
