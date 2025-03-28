namespace TimeSnapBackend_MySql.Dtos
{
    public class TimesheetDto
    {
        public string? EmpId { get; set; }
        public string? TaskId { get; set; }
        public DateTime? Date { get; set; }
        public double TotalMinutes { get; set; }
        public string? Notes { get; set; }
    }
}
