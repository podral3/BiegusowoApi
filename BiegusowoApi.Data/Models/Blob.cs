using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BiegusowoApi.Data.Models;

public class Blob
{
    public Guid Id { get; set; }
    public string StorageKey { get; set; }
    public string ContentType { get; set; }
    public string Bucket { get; set; }
    public bool Uploaded { get; set; } = false;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
