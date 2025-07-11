namespace QuickFixApi.Models
{
    public class Provider
    {
        public int Id { get; set; } // ID del proveedor
        public string Name { get; set; } = null!;
        public string Profession { get; set; } = null!;
        public double Rating { get; set; }
        public int Reviews { get; set; }
        public string Location { get; set; } = null!;
        public string Price { get; set; } = null!;
        public string Image { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string[] Services { get; set; } = []; // Array de strings
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Availability { get; set; } = null!; // JSON o string de slots
        public string Certifications { get; set; } = null!;
        public string Coordinates { get; set; } = null!;
    }
}
