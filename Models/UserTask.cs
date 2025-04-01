//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace TimeSnapBackend_MySql.Models
//{
//    [Table("UserTasks")]
//    public class UserTask
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required]
//        [Column(TypeName = "VARCHAR(50)")]
//        public string TaskId { get; set; } = string.Empty; // FK to TaskModel.TaskId

//        [ForeignKey("TaskId")]
//        public virtual TaskModel? Task { get; set; }

//        [Required]
//        [Column(TypeName = "VARCHAR(50)")]
//        public string EmpId { get; set; } = string.Empty; // FK to AppUser.EmpId

//        [ForeignKey("EmpId")]
//        public virtual AppUser? Employee { get; set; }

//        [Column(TypeName = "TINYINT")]
//        public TaskStatus Status { get; set; } = TaskStatus.NotStarted;

//        [Column(TypeName = "DATETIME(6)")]
//        public DateTime? CompletedDate { get; set; }
//    }
//}





using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeSnapBackend_MySql.Models
{
    public enum TasksStatus
    {
        NotStarted,
        InProgress,
        Completed
    }

    [Table("UserTasks")]
    public class UserTask
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string TaskId { get; set; } = string.Empty; // FK to TaskModel.TaskId

        [ForeignKey("TaskId")]
        public virtual TaskModel? Task { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string EmpId { get; set; } = string.Empty; // FK to AppUser.EmpId

        [ForeignKey("EmpId")]
        public virtual AppUser? Employee { get; set; }

        [Column(TypeName = "TINYINT")]
        public TasksStatus Status { get; set; } = TasksStatus.NotStarted; // User-specific Status

        [Column(TypeName = "DATETIME(6)")]
        public DateTime? CompletedDate { get; set; }
    }
}
