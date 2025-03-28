namespace TimeSnapBackend_MySql.Models
{
    public class UpdateTimeSheetRequest
    {
        public DateTime Date { get; set; }
        public double TotalMinutes { get; set; }
        public string? Notes { get; set; }

    }
}
