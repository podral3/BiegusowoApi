namespace BiegusowoApi.Data.Models;

public class Species
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}