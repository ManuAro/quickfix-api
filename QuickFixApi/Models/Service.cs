namespace QuickFixApi.Models
{
    public class Service
    {
        public int Id { get; set; } // ID del servicio

        public string Name { get; set; } = null!; // Ej: "Instalaciones eléctricas"
        public string Description { get; set; } = null!; // Detalle del servicio

        public int WorkerId { get; set; } // ID del proveedor asociado
        public string Category { get; set; } = null!; // Ej: "Electricidad", "Plomería", etc.
    }
}
