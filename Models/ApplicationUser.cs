using Microsoft.AspNetCore.Identity;

namespace FerienspassWebApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Vorname { get; set; }
        public string? Nachname { get; set; }
        public string? Strasse {  get; set; }
        public string? PLZ { get; set; }
        public string? Ort { get; set; }
        public string? Telefonnummer { get; set; }
        public List<EmergencyContact> Contacts { get; set; } = new();
    }
}
