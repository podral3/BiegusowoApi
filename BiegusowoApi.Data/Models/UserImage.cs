using BiegusowoApi.Data.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace BiegusowoApi.Data.Models;

public class UserImage
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string Bucket { get; set; }
    public int FileSizeBytes { get; set; }
    public UserImageType Type { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
