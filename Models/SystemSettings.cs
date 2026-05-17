namespace FerienspassWebApp.Models
{
    public class SystemSettings
    {
        public int Id { get; set; }

        //Kursleiter Anmeldefenster
        public DateTime? KursleiterStart { get; set; }
        public DateTime? KursleiterEnde { get; set;}

        //Eltern Anmeldefenster
        public DateTime? AnmeldungStart { get; set; }
        public DateTime? AnmeldungEnde { get; set; }
    }
}
