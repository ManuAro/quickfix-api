using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickFixApi.Models;

[Table("users")]
public class User
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } // Cambiado a Guid

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("email")]
    public string Email { get; set; } = null!;

    [Column("password")]
    public string Password { get; set; } = null!;

    [Column("user_type")]
    public string UserType { get; set; } = null!; // Ej: "client" o "provider"

    [Column("profession")]
    public string Profession { get; set; } = null!;

    [Column("approved")]
    public bool Approved { get; set; } = true; // Por defecto true para clientes
}
