namespace TimeSnapBackend_MySql.Dtos
{
    public class TaskCreateDto
    {
        public string? TaskId { get; set; }
        public string? TaskName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? BillingType { get; set; }
        public List<AssigneeDto>? Assignee { get; set; }
    }

    public class AssigneeDto
    {
        public string? EmpId { get; set; }
        public string? Assignee { get; set; }
    }

}
