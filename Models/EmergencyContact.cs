namespace FerienspassWebApp.Models
{
    public class EmergencyContact
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;
        public string Telefon { get; set; } = default!;
        public string Beziehung { get; set; }

        public string ApplicationUserId { get; set; } = "";
        public ApplicationUser ApplicationUser { get; set; } = null!;
    }
}
