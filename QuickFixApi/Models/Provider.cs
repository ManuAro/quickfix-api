using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickFixApi.Models;

[Table("providers")]
public class Provider
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("profession")]
    public string Profession { get; set; } = null!;

    [Column("rating")]
    public double Rating { get; set; }

    [Column("reviews")]
    public int Reviews { get; set; }

    [Column("location")]
    public string Location { get; set; } = null!;

    [Column("price")]
    public string Price { get; set; } = null!;

    [Column("image")]
    public string Image { get; set; } = null!;

    [Column("description")]
    public string Description { get; set; } = null!;

    [NotMapped]
    public string[] Services { get; set; } = [];

    [Column("services")]
    public string ServicesRaw
    {
        get => string.Join(",", Services);
        set => Services = string.IsNullOrEmpty(value) ? [] : value.Split(',');
    }

    [Column("phone")]
    public string Phone { get; set; } = null!;

    [Column("email")]
    public string Email { get; set; } = null!;

    [Column("availability")]
    public string Availability { get; set; } = null!;

    [Column("certifications")]
    public string Certifications { get; set; } = null!;

    [Column("coordinates")]
    public string Coordinates { get; set; } = null!;
}
