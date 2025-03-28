namespace TimeSnapBackend_MySql.Dtos
{
    public class TimesheetResponseDto
    {
        public string? UserName { get; set; }
        public string? EmpId { get; set; }
        public string? TaskId { get; set; }

        public string? TaskName { get; set; }
        public DateTime Date { get; set; }
        public string? BillingType { get; set; }
        public int TimesheetId { get; set; }
        public double TotalMinutes { get; set; }
        public string? Notes { get; set; }
    }

}
