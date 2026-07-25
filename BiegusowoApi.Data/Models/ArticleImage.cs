using System.ComponentModel.DataAnnotations.Schema;

namespace BiegusowoApi.Data.Models;

public class ArticleImage
{
    public Guid Id { get; set; }

    [ForeignKey(nameof(Article))]
    public Guid ArticleId { get; set; }
    public Article Article { get; set; } = null!;
    
    public string FileName { get; set; }
    public short Order { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DeletedAt { get; set; }
}