namespace TimeSnapBackend_MySql.Models
{
    public class Leave
    {
        public string? EmployeeName { get; set; }
        public DateTime LeaveStartDate { get; set; }
        public int NumberOfDays { get; set; }
    }

}
