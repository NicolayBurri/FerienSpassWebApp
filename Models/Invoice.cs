namespace FerienspassWebApp.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        public string ParentUserId { get; set; } = default!;

        public ApplicationUser ParentUser { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Reference { get; set; } = default!;

        public decimal TotalAmount { get; set; }

        public bool IsPaid { get; set; } = false;

        public List<InvoiceItem> Items { get; set; } = new();

        public InvoiceStatus Status { get; set; } = InvoiceStatus.Open;

        public DateTime? PaidAt { get; set; }

        public DateTime? ReminderSentAt { get; set; }
    }
}