using System.ComponentModel.DataAnnotations;

namespace FerienspassWebApp.Models
{
    public class EventRoleAssignment
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; } = default!;

        public string UserId { get; set; } = default;

        public ApplicationUser User { get; set; } = null!;

        public RoleType Role {  get; set; }

        public int? Seats { get; set; }
    }

    public enum RoleType
    {
        Driver,
        Helper
    }
}
