namespace BiegusowoApi.Data.Models
{
    public class Breed
    {
        public int Id { get; set; }
        
        public int SpeciesId { get; set; }
        public Species Species { get; set; } = null!;

        public string Name { get; set; }
        public string Slug { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
