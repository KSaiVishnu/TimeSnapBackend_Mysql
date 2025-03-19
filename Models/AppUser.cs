using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TimeSnapBackend_MySql.Models
{
    public class AppUser : IdentityUser
    {
        [PersonalData]
        public string? FullName { get; set; }

        [Required]
        public string? EmpId { get; set; }

        [JsonIgnore]
        public virtual ICollection<Timesheet>? Timesheets { get; set; }
    }
}
