namespace TimeSnapBackend_MySql.Dtos
{
    public class TaskUpdateDto
    {
        public string TaskID { get; set; } = "";
        public string Task { get; set; } = "";
        public string BillingType { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Assignee { get; set; } = "";
        public string EmpId { get; set; } = "";
    }


}
