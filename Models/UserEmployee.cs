using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeSnapBackend_MySql.Models
{
    public class UserEmployee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string? EmployeeId { get; set; } // Employee ID

        [Required]
        [Column(TypeName = "VARCHAR(255)")]
        public string? UserName { get; set; } // Assignee Name

        [Column(TypeName = "VARCHAR(320)")]
        public string? Email { get; set; }
    }
}
