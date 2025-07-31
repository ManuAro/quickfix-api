using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickFixApi.Models;

[Table("services")]
public class Service
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("description")]
    public string Description { get; set; } = null!;

    [Column("worker_id")]
    public Guid WorkerId { get; set; } // 👈 Cambiado a Guid (usás uuid en Supabase)

    [Column("category")]
    public string Category { get; set; } = null!;
}
