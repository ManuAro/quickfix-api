namespace QuickFixApi.Models
{
    public class Slot
    {
        public int Id { get; set; } 
        public string Time { get; set; } = null!;   // Ej: "14:00"
        public bool Available { get; set; }         // true si está libre
        public bool Booked { get; set; }            // true si ya fue reservado
    }

    public class Availability
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }         // ID del proveedor

        public DateTime Date { get; set; }          // Día disponible
        public List<Slot> Slots { get; set; } = new(); // Lista de horarios (jsonb en Supabase)
    }
}
