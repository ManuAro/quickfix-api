using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickFixApi.Models;

[Table("reviews")]
public class Review
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } // Supabase usa uuid

    [Column("appointment_id")]
    public Guid AppointmentId { get; set; } // Asumiendo que appointments.id es uuid

    [Column("provider_id")]
    public Guid ProviderId { get; set; }

    [Column("client_id")]
    public Guid ClientId { get; set; }

    [Column("rating")]
    public int Rating { get; set; } // de 1 a 5

    [Column("comment")]
    public string? Comment { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
