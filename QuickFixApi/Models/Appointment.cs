namespace QuickFixApi.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public int ClientId { get; set; }

        public string ProviderName { get; set; } = null!;
        public string ClientName { get; set; } = null!;
        public string ProviderProfession { get; set; } = null!;

        public DateTime Date { get; set; }
        public string Time { get; set; } = null!;

        public string Status { get; set; } = "pending"; // pending / confirmed / cancelled, etc.
        public string Location { get; set; } = null!;
        public string Notes { get; set; } = null!;

        // ✅ Campos nuevos
        public bool? AcceptedByProvider { get; set; } // null = no respondió aún
        public DateTime? EndTime { get; set; }         // solo si acepta
    }
}
