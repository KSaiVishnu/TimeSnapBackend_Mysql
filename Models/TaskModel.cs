using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeSnapBackend_MySql.Models
{
    public enum TaskStatus
    {
        NotStarted,
        InProgress,
        Completed
    }

    public class TaskModel
    {
        [Key]
        public int Id { get; set; }

        public string? TaskID { get; set; }
        public string? Task { get; set; }
        public string? Assignee { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }

        public TaskStatus Status { get; set; } = TaskStatus.NotStarted; // Enum stored as int

        public string? BillingType { get; set; }
        public string? EmpId { get; set; }
    }
}
