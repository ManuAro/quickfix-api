using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickFixApi.Models;

[Table("applications")]
public class Application
{
    [Key]
    [Column("id")]
    public int Id { get; set; } // Está bien que sea int si en Supabase es serial

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("phone")]
    public string Phone { get; set; } = string.Empty;

    [Column("profession")]
    public string Profession { get; set; } = string.Empty;

    [Column("other_profession")]
    public string? OtherProfession { get; set; }

    [Column("city")]
    public string City { get; set; } = string.Empty;

    [Column("other_city")]
    public string? OtherCity { get; set; }

    [Column("experience")]
    public string Experience { get; set; } = string.Empty;

    [Column("about")]
    public string About { get; set; } = string.Empty;

    [Column("has_certifications")]
    public bool HasCertifications { get; set; }

    [Column("has_tools")]
    public bool HasTools { get; set; }

    [Column("accept_terms")]
    public bool AcceptTerms { get; set; }

    [Column("status")]
    public string Status { get; set; } = "pending";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
