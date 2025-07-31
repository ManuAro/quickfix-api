using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace QuickFixApi.Models;

public class Slot
{
    [Key]
    [Column("id")]
    public int Id { get; set; } // Podés mantenerlo como int si no lo usás para relaciones

    [Column("time")]
    public string Time { get; set; } = null!; // Ej: "14:00"

    [Column("available")]
    public bool Available { get; set; }

    [Column("booked")]
    public bool Booked { get; set; }
}

[Table("availability")]
public class Availability
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("provider_id")]
    public Guid ProviderId { get; set; }

    [Column("date")]
    public DateTime Date { get; set; }

    [NotMapped]
    public List<Slot> Slots { get; set; } = new();

    [Column("slots")]
    public string SlotsJson
    {
        get => JsonSerializer.Serialize(Slots);
        set => Slots = string.IsNullOrEmpty(value)
            ? new List<Slot>()
            : JsonSerializer.Deserialize<List<Slot>>(value)!;
    }
}
