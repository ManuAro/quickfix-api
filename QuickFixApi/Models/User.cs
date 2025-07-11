namespace QuickFixApi.Models
{
    public class User
    {
        public int Id { get; set; } // ID único
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string UserType { get; set; } = null!; // Ej: "client" o "provider"
        public string Profession { get; set; } = null!;
    }
}
