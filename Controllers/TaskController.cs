using TimeSnapBackend_MySql.Dtos;
using TimeSnapBackend_MySql.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TimeSnapBackend_MySql.Controllers
{
    public class TaskStatus
    {
        public int Completed { get; set; }
        public int InProgress { get; set; }
        public int Pending { get; set; }
    }
    public class Assignee
{
    public string? AssigneeName { get; set; }
    public string? EmpId { get; set; }
}

public class UpdateTaskRequest
{
    public List<Assignee>? Assignees { get; set; }
}



    [Route("api/tasks")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskController(AppDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadTasks([FromBody] List<TaskModel> tasks)
        {
            if (tasks == null || tasks.Count == 0)
            {
                return BadRequest(new { message = "Invalid file data" });
            }

            foreach (var task in tasks)
            {
                _context.Tasks.Add(task);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Data inserted successfully" });
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] string? billingType)
        {
            var tasksQuery = _context.Tasks.AsQueryable();

            if (!string.IsNullOrEmpty(billingType))
            {
                tasksQuery = tasksQuery.Where(t => t.BillingType == billingType);
            }

            var tasks = await tasksQuery.ToListAsync();
            return Ok(tasks);
        }





        [AllowAnonymous]
        [HttpGet("details/{Id}")]
        public async Task<IActionResult> GetTaskDetailsById(int Id)
        {
            var task = await _context.Tasks.FindAsync(Id);

            if (task == null)
            {
                return NotFound(new { message = "Task not found" });
            }

            return Ok(task);
        }




        [AllowAnonymous]
        [HttpGet("users/{taskId}")]
        public async Task<IActionResult> GetUsersByTaskId(string taskId)
        {
            var allTasksWithId = await _context.Tasks
                .Where(t => t.TaskID == taskId)
                .ToListAsync();

            if (!allTasksWithId.Any())
            {
                return Ok(Array.Empty<Task>()); // More efficient for empty arrays
            }

            // Group and merge the data
            var mergedTask = allTasksWithId
                .GroupBy(t => new { t.TaskID, t.Task, t.StartDate, t.DueDate, t.BillingType })
                .Select(g => new
                {
                    g.Key.TaskID,
                    g.Key.Task,
                    g.Key.StartDate,
                    g.Key.DueDate,
                    //g.Key.Status,
                    g.Key.BillingType,
                    Assignee = new
                    {
                        Assignee = g.Select(t => t.Assignee).ToList(),
                        EmpId = g.Select(t => t.EmpId).ToList()
                    }
                })
                .FirstOrDefault();

            return Ok(mergedTask);

        }





        [AllowAnonymous]
        [HttpGet("status/{empId}")]
        public async Task<IActionResult> GetTaskStatus(string empId)
        {
            var filteredTasks = await _context.Tasks.Where(t => t.EmpId == empId).ToListAsync();

            var taskStatusCounts = filteredTasks
                .GroupBy(t => t.Status)
                .Select(g => new
                {
                    Status = g.Key.ToString(), // Convert Enum to string
                    Count = g.Count()
                })
                .ToList();

            return Ok(taskStatusCounts);
        }

        [AllowAnonymous]
        [HttpGet("{empId}")]
        public async Task<IActionResult> GetTasksByEmpId(string empId)
        {
            if (string.IsNullOrEmpty(empId))
            {
                return BadRequest(new { message = "Employee Id is Required" });
            }
            //var tasks = await _context.Tasks.ToListAsync();
            var tasks = await _context.Tasks.Where(t => t.EmpId!.Equals(empId)).ToListAsync();
            var groupedTasks = tasks.GroupBy(t => t.Status).Select(g => new
            {
                Status = g.Key.ToString(),
                Count = g.Count(),
                Tasks = g.ToList(),
            }).ToList();


            return Ok(groupedTasks);
        }

        [AllowAnonymous]
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateTaskById(int Id, [FromBody] TaskModel updatedTask)
        {
            if (Id != updatedTask.Id)
                return BadRequest("Task ID mismatch");

            var existingTask = await _context.Tasks.FindAsync(Id);
            if (existingTask == null)
                return NotFound();

            existingTask.Status = updatedTask.Status;

            await _context.SaveChangesAsync();
            return Ok(existingTask);
        }


        [AllowAnonymous]
        [HttpGet("{taskId}/timesheets")]
        public async Task<IActionResult> GetTimesheetsByTaskId(int taskId)
        {
            var timesheets = await _context.Timesheets
                                           .Where(t => t.TaskId == taskId)
                                           .ToListAsync();



            if (timesheets == null || !timesheets.Any())
            {
                return NotFound(new { message = "No timesheets found for this task!" });
            }

            return Ok(timesheets);
        }


        [AllowAnonymous]
        [HttpPut("update-task/{taskID}")]
        public async Task<IActionResult> UpdateTaskAssignees(string taskID, [FromBody] UpdateTaskRequest request)
        {
            var taskRecords = await _context.Tasks
                .Where(t => t.TaskID == taskID)
                .ToListAsync();

            if (!taskRecords.Any()) return NotFound("Task not found.");

            var existingAssignees = taskRecords.Select(t => t.EmpId).ToHashSet();

            // Find new assignees (not already in existing tasks)
            var newAssignees = request.Assignees!
                .Where(a => !existingAssignees.Contains(a.EmpId))
                .ToList();

            foreach (var assignee in newAssignees)
            {
                if (!string.IsNullOrEmpty(assignee.EmpId))
                {
                    _context.Tasks.Add(new TaskModel
                    {
                        TaskID = taskID,
                        Task = taskRecords.First().Task,
                        Assignee = assignee.AssigneeName,
                        EmpId = assignee.EmpId,
                        StartDate = taskRecords.First().StartDate,
                        DueDate = taskRecords.First().DueDate,
                        BillingType = taskRecords.First().BillingType,
                        Status = 0
                    });
                }
            }

            // Find assignees to remove (not in request)
            var assigneesToRemove = taskRecords
                .Where(t => !request.Assignees!.Any(a => a.EmpId == t.EmpId))
                .ToList();

            _context.Tasks.RemoveRange(assigneesToRemove);

            await _context.SaveChangesAsync();
            return Ok(new { message = "Task Updated Successfully" });
        }










        [HttpPut("update-task")]
        public async Task<IActionResult> UpdateTotalTask([FromBody] List<TaskUpdateDto> updatedTasks)
        {
            if (updatedTasks == null || updatedTasks.Count == 0)
                return BadRequest("Invalid task data");

            string taskId = updatedTasks.First().TaskID;

            // Get existing task records for this TaskID
            var existingTasks = await _context.Tasks
                .Where(t => t.TaskID == taskId)
                .ToListAsync();

            // Convert existing tasks to a dictionary for quick lookup
            //var existingTaskMap = existingTasks.ToDictionary(t => t.EmpId, t => t);

             var existingTaskMap = existingTasks
                .Where(t => !string.IsNullOrEmpty(t.EmpId)) // Avoid null or empty keys
                .GroupBy(t => t.EmpId)  // Ensure uniqueness
                .ToDictionary(g => g.Key, g => g.First());



            foreach (var updatedTask in updatedTasks)
            {
                if (existingTaskMap.ContainsKey(updatedTask.EmpId))
                {
                    // Update existing task details
                    var taskToUpdate = existingTaskMap[updatedTask.EmpId];
                    taskToUpdate.Task = updatedTask.Task;
                    taskToUpdate.BillingType = updatedTask.BillingType;
                    taskToUpdate.StartDate = updatedTask.StartDate;
                    taskToUpdate.DueDate = updatedTask.DueDate;

                    _context.Tasks.Update(taskToUpdate);
                    existingTaskMap.Remove(updatedTask.EmpId); // Mark as processed
                }
                else
                {
                    // Insert new assignee
                    _context.Tasks.Add(new TaskModel
                    {
                        TaskID = updatedTask.TaskID,
                        Task = updatedTask.Task,
                        BillingType = updatedTask.BillingType,
                        StartDate = updatedTask.StartDate,
                        DueDate = updatedTask.DueDate,
                        Assignee = updatedTask.Assignee,
                        EmpId = updatedTask.EmpId
                    });
                }
            }

            // Remove assignees that were not in the updated list
            _context.Tasks.RemoveRange(existingTaskMap.Values);

            await _context.SaveChangesAsync();
            //return Ok("Task updated successfully");
            return Ok(new { message = "Task Updated Successfully" });

        }











        [AllowAnonymous]
        [HttpDelete("delete-task/{taskId}")]
        public async Task<IActionResult> DeleteTask(string taskId)
        {
            //var taskRecords = await _context.Tasks.Where(t => t.TaskID == taskID).ToListAsync();
            var taskRecords = await _context.Tasks.Where(t => t.TaskID == taskId).ToListAsync();
            foreach(var task in taskRecords)
            {
                _context.Tasks.Remove(task);
            }


            await _context.SaveChangesAsync();
            //return Ok(taskRecords);
            return Ok(new { message = "Task Deleted Successfully" });
        }

    


    
    
    }




}