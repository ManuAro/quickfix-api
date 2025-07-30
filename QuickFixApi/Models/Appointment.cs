using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickFixApi.Models;

[Table("appointments")]
public class Appointment
{
    [Key]
    public int Id { get; set; }

    [Column("provider_id")]
    public int ProviderId { get; set; }

    [Column("client_id")]
    public int ClientId { get; set; }

    [Column("provider_name")]
    public string ProviderName { get; set; } = null!;

    [Column("client_name")]
    public string ClientName { get; set; } = null!;

    [Column("provider_profession")]
    public string ProviderProfession { get; set; } = null!;

    [Column("date")]
    public string Date { get; set; } = null!;

    [Column("time")]
    public string Time { get; set; } = null!;

    [Column("status")]
    public string Status { get; set; } = "pending";

    [Column("location")]
    public string Location { get; set; } = null!;

    [Column("notes")]
    public string Notes { get; set; } = null!;

    [Column("accepted_by_provider")]
    public bool? AcceptedByProvider { get; set; }

    [Column("end_time")]
    public DateTime? EndTime { get; set; }

    [Column("price")]
    public string? Price { get; set; }

    [Column("service_description")]
    public string? ServiceDescription { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
    
}

