namespace BiegusowoApi.Domain.Blobs;

public static class KeyGenerator
{
    public static string GenerateKey(EntityType entityType, string ownerId, string fileName, bool? avatar = false)
    {
        if (entityType.Equals(EntityType.User))
        {
            if (avatar.HasValue && avatar.Value)
            {
                return $"{entityType}s/{ownerId}/avatar_{fileName}";
            }
            return $"{entityType}s/{ownerId}/background_{fileName}";
        }
        return $"{entityType}s/{ownerId}/{fileName}";


    }
}
