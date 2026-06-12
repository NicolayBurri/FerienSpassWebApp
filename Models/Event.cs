using System.ComponentModel.DataAnnotations;

namespace FerienspassWebApp.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        public string Eventname { get; set; }

        public List<EventKursleiter> Kursleiter { get; set; } = new();

        public EventKategorie Kategorie { get; set; }

        public string? Beschreibung {  get; set; }

        public string Location { get; set; }
        public string Treffpunkt { get; set; }

        public int? price { get; set; }


        public int MaxKinder { get; set; }
        public int MinKinder { get; set; }

        public int WaitlisteLimit { get; set; } = 3;

        public bool IsFull { get; set; }

        public bool BrauchtBetreuung {  get; set; }
        public int? AnzahlBetreuung { get; set; }

        public bool BrauchtFahrer {  get; set; }
        public int? AnzahlSitzplaetze { get; set; }

        public int? AlterMin {  get; set; }
        public int? AlterMax { get; set; }

        public DateTime? StartZeit {  get; set; }
        public DateTime? EndeZeit {  get; set; }

        public string? BildUrl { get; set; }

        public string? DocumentUrl { get; set; }

        public bool IsArchived { get; set; }

        public int? CopiedFromEventId { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        public List<EventRoleAssignment> EventRoleAssignments { get; set; } = new();
    }
}
