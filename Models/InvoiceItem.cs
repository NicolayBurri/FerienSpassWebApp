namespace FerienspassWebApp.Models
{
    public class InvoiceItem
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; } = default!;

        public int EventId { get; set; }
        public Event Event { get; set; } = default!;

        public int ChildId { get; set; } = default!;

        public decimal Price { get; set; }
    }
}
