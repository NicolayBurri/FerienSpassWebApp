namespace FerienspassWebApp.Models
{
    public class RefundRequest
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; } = null!;

        public int EventId { get; set; }
        public string EventName { get; set; } = "";

        public int ChildId { get; set; }
        public string ChildName { get; set; } = "";

        public int? Amount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsRefundet { get; set; }
    }
}
