using System.ComponentModel.DataAnnotations;

namespace TimeSnapBackend_MySql.Dtos
{
    public class UserEmployeeDto
    {
        [Required]
        public string? EmployeeId { get; set; } // Employee ID

        [Required]
        public string? UserName { get; set; } // Assignee Name
    }
}
