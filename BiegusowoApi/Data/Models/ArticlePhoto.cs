using System.ComponentModel.DataAnnotations.Schema;

namespace BiegusowoApi.Data.Models;

public class ArticlePhoto
{
    public int Id { get; set; }

    [ForeignKey(nameof(Article))]
    public int ArticleId { get; set; }
    public Article Article { get; set; } = null!;
    
    public string StorageKey { get; set; }
    public short Order { get; set; } = 0;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}