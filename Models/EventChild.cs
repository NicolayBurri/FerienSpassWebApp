namespace FerienspassWebApp.Models
{
    public class EventChild
    {
        public int Id { get; set; }
        public int EventId { get; set; }

        public Event Event { get; set; } = default!;

        public int ChildId { get; set; }
        public Child Child { get; set; } = default!;

        public DateTime RegisteredAt { get; set; } = DateTime.Now;

        public EnrollmentStatus Status { get; set; }

        public bool IsInvoiced { get; set; }
    }
}
