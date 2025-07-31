using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickFixApi.Models;

[Table("appointments")]
public class Appointment
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("provider_id")]
    public Guid ProviderId { get; set; }

    [Column("client_id")]
    public Guid ClientId { get; set; }

    [Column("provider_name")]
    public string ProviderName { get; set; } = null!;

    [Column("client_name")]
    public string ClientName { get; set; } = null!;

    [Column("client_email")]
    public string ClientEmail { get; set; } = null!; // 🆕 Para validación de reviews

    [Column("provider_profession")]
    public string ProviderProfession { get; set; } = null!;

    [Column("date")]
    public string Date { get; set; } = null!;

    [Column("time")]
    public string Time { get; set; } = null!;

    [Column("status")]
    public string Status { get; set; } = "pending"; // 🆕 accepted / completed / cancelled

    [Column("accepted_by_provider")]
    public bool AcceptedByProvider { get; set; } = false;

    [Column("end_time")]
    public string? EndTime { get; set; }

    [Column("price")]
    public string? Price { get; set; }

    [Column("service_description")]
    public string? ServiceDescription { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
