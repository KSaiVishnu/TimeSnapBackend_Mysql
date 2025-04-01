using System.Text.Json.Serialization;
using TimeSnapBackend_MySql.Services;

namespace TimeSnapBackend_MySql.Dtos
{
    //public class TimesheetUploadDto
    //{
    //        public string? EmpId { get; set; }
    //        public string? TaskId { get; set; }
    //        public DateTime Date { get; set; }
    //        public string? TotalHours { get; set; } // "1:00"
    //        public string? Notes { get; set; }
    //    }



    public class TimesheetUploadDto
    {
        public string? UserId { get; set; }  // Email from Excel
        public string? TaskId { get; set; }

        [JsonPropertyName("Date")]
        [JsonConverter(typeof(JsonDateConverter))] // Custom converter
        public DateTime Date { get; set; }
        public string? TotalHours { get; set; }
        public string? Notes { get; set; }
    }


}
