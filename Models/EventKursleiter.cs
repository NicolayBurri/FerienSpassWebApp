namespace FerienspassWebApp.Models
{
    public class EventKursleiter
    {
        public int EventId {  get; set; }
        public Event Event { get; set; } = default!;

        public string UserId { get; set; } = default!;

        public ApplicationUser User { get; set; } = default!;
    }
}
