using TimeSnapBackend_MySql.Models;

namespace TimeSnapBackend_MySql.Dtos
{
    public class UpdateTaskDto
    {
        public TasksStatus Status { get; set; }
        public DateTime? CompletedDate { get; set; }
    }

}
