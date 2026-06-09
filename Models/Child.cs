namespace FerienspassWebApp.Models
{
    public class Child
    {
        public int Id { get; set; }

        public string ParentUserId { get; set; } = default!;

        public ApplicationUser? ParentUser { get; set; }

        public string Name { get; set; } = string.Empty;
        public int Alter {  get; set; }

        public KlasseStufe Klasse { get; set; }

        public string Allergien {  get; set; } = string.Empty ;
        public string Besonderes {  get; set; } = string.Empty ;

        public string UserId { get; set; } = string.Empty;
    }
}
