//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace TimeSnapBackend_MySql.Models
//{
//    public enum TaskStatus
//    {
//        NotStarted,
//        InProgress,
//        Completed
//    }

//    public class TaskModel
//    {
//        [Key]
//        public int Id { get; set; }

//        public string? TaskID { get; set; }
//        public string? Task { get; set; }
//        public string? Assignee { get; set; }
//        public DateTime StartDate { get; set; }
//        public DateTime DueDate { get; set; }

//        public TaskStatus Status { get; set; } = TaskStatus.NotStarted; // Enum stored as int

//        public string? BillingType { get; set; }
//        public string? EmpId { get; set; }
//    }
//}



//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Collections.Generic;

//namespace TimeSnapBackend_MySql.Models
//{
//    public enum TaskStatus
//    {
//        NotStarted,
//        InProgress,
//        Completed
//    }

//    [Table("Tasks")]
//    public class TaskModel
//    {
//        [Key]
//        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public int Id { get; set; } // Primary Key

//        [Required]
//        [Column(TypeName = "VARCHAR(50)")]
//        public string TaskId { get; set; } = string.Empty; // Now references TaskId, not Id

//        [Required]
//        [Column(TypeName = "VARCHAR(255)")]
//        public string TaskName { get; set; } = string.Empty;

//        [Column(TypeName = "DATETIME(6)")]
//        public DateTime StartDate { get; set; }

//        [Column(TypeName = "DATETIME(6)")]
//        public DateTime DueDate { get; set; }

//        [Column(TypeName = "TINYINT")]
//        public TaskStatus Status { get; set; } = TaskStatus.NotStarted;

//        [Column(TypeName = "VARCHAR(50)")]
//        public string BillingType { get; set; } = string.Empty;

//        // Navigation Property (Many-to-Many)
//        public virtual ICollection<UserTask> UserTasks { get; set; } = new List<UserTask>();
//    }
//}




using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace TimeSnapBackend_MySql.Models
{
    [Table("Tasks")]
    public class TaskModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // Auto-generated Primary Key

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string TaskId { get; set; } = string.Empty; // Unique Task Identifier

        [Required]
        [Column(TypeName = "VARCHAR(255)")]
        public string TaskName { get; set; } = string.Empty;

        [Column(TypeName = "DATETIME(6)")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "DATETIME(6)")]
        public DateTime DueDate { get; set; }

        [Column(TypeName = "VARCHAR(50)")]
        public string BillingType { get; set; } = string.Empty;

        // Navigation Property (Many-to-Many)
        public virtual ICollection<UserTask> UserTasks { get; set; } = new List<UserTask>();
    }
}

