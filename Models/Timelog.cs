namespace TimeSnapBackend_MySql.Models
{
    public class Timelog
    {
        public string? User { get; set; }
        public TimeSpan DailyLog { get; set; }
        public DateTime Date { get; set; }
    }

}
