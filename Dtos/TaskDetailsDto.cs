namespace TimeSnapBackend_MySql.Dtos
{
    public class TaskDetailsDto
    {
        public string? TaskId { get; set; }
        public string? TaskName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? BillingType { get; set; }
        public List<EmployeeDto>? AssignedEmployees { get; set; }
    }

    public class EmployeeDto
    {
        public string? EmpId { get; set; }
        public string? Assignee { get; set; }
    }

}
