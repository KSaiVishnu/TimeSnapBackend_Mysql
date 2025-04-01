namespace TimeSnapBackend_MySql.Dtos
{
    public class TaskCompletionDto
    {
        public string? EmployeeId { get; set; }
        public string? UserName { get; set; }
        public string? TaskId { get; set; }
        public string? TaskName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CompletedDate { get; set; }
        public double TotalHours { get; set; }
        public string? BillingType { get; set; }

    }

}
