using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TimeSnapBackend_MySql.Models
{
    public class Timesheet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string? EmpId { get; set; }  // FK to AppUser.EmpId

        [ForeignKey("EmpId")]
        [JsonIgnore]
        public virtual AppUser? Employee { get; set; }

        [Required]
        public int TaskId { get; set; } // FK to TaskModel.Id

        [ForeignKey("TaskId")]
        public virtual TaskModel? Task { get; set; }

        // Correct MySQL datetime type
        [Column(TypeName = "datetime(6)")]
        public DateTime? Date { get; set; }

        public double TotalMinutes { get; set; }

        public string? Notes { get; set; }
    }
}
