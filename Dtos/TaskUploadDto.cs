namespace TimeSnapBackend_MySql.Dtos
{
    public class TaskUploadDto
    {
        public string Task { get; set; } = string.Empty; // Maps to TaskModel.TaskName
        public string TaskID { get; set; } = string.Empty; // Not used in TaskModel (can be stored if needed)
        public string Assignee { get; set; } = string.Empty; // Not used (only for reference)
        public string EmpId { get; set; } = string.Empty; // Maps to UserTask.EmpId
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public string BillingType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty; // Not stored
    }

}
