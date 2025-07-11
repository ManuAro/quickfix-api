namespace QuickFixApi.Models
{
    public class Application
    {
        public int Id { get; set; }

        // Datos del usuario que aplica
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        // Profesión
        public string Profession { get; set; } = string.Empty;
        public string? OtherProfession { get; set; }

        // Ubicación
        public string City { get; set; } = string.Empty;
        public string? OtherCity { get; set; }

        // Experiencia y descripción
        public string Experience { get; set; } = string.Empty;
        public string About { get; set; } = string.Empty;

        // Flags de certificación, herramientas y aceptación
        public bool HasCertifications { get; set; }
        public bool HasTools { get; set; }
        public bool AcceptTerms { get; set; }

        // Estado de la solicitud
        public string Status { get; set; } = "pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
