﻿//using TimeSnapBackend_MySql.Dtos;
//using TimeSnapBackend_MySql.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace TimeSnapBackend_MySql.Controllers
//{
//    public class TaskStatus
//    {
//        public int Completed { get; set; }
//        public int InProgress { get; set; }
//        public int Pending { get; set; }
//    }
//    public class Assignee
//    {
//        public string? AssigneeName { get; set; }
//        public string? EmpId { get; set; }
//    }

//    public class UpdateTaskRequest
//    {
//        public List<Assignee>? Assignees { get; set; }
//    }



//    [Route("api/tasks")]
//    [ApiController]
//    public class TaskController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public TaskController(AppDbContext context)
//        {
//            _context = context;
//        }

//        [AllowAnonymous]
//        [HttpPost("upload")]
//        public async Task<IActionResult> UploadTasks([FromBody] List<TaskModel> tasks)
//        {
//            if (tasks == null || tasks.Count == 0)
//            {
//                return BadRequest(new { message = "Invalid file data" });
//            }

//            foreach (var task in tasks)
//            {
//                _context.Tasks.Add(task);
//            }

//            await _context.SaveChangesAsync();
//            return Ok(new { message = "Data inserted successfully" });
//        }


//        [AllowAnonymous]
//        [HttpGet]
//        public async Task<IActionResult> GetTasks([FromQuery] string? billingType)
//        {
//            var tasksQuery = _context.Tasks.AsQueryable();

//            if (!string.IsNullOrEmpty(billingType))
//            {
//                tasksQuery = tasksQuery.Where(t => t.BillingType == billingType);
//            }

//            var tasks = await tasksQuery.ToListAsync();
//            return Ok(tasks);
//        }





//        [AllowAnonymous]
//        [HttpGet("details/{Id}")]
//        public async Task<IActionResult> GetTaskDetailsById(int Id)
//        {
//            var task = await _context.Tasks.FindAsync(Id);

//            if (task == null)
//            {
//                return NotFound(new { message = "Task not found" });
//            }

//            return Ok(task);
//        }




//        [AllowAnonymous]
//        [HttpGet("users/{taskId}")]
//        public async Task<IActionResult> GetUsersByTaskId(string taskId)
//        {
//            var allTasksWithId = await _context.Tasks
//                .Where(t => t.TaskID == taskId)
//                .ToListAsync();

//            if (!allTasksWithId.Any())
//            {
//                return Ok(Array.Empty<Task>()); // More efficient for empty arrays
//            }

//            // Group and merge the data
//            var mergedTask = allTasksWithId
//                .GroupBy(t => new { t.TaskID, t.Task, t.StartDate, t.DueDate, t.BillingType })
//                .Select(g => new
//                {
//                    g.Key.TaskID,
//                    g.Key.Task,
//                    g.Key.StartDate,
//                    g.Key.DueDate,
//                    //g.Key.Status,
//                    g.Key.BillingType,
//                    Assignee = new
//                    {
//                        Assignee = g.Select(t => t.Assignee).ToList(),
//                        EmpId = g.Select(t => t.EmpId).ToList()
//                    }
//                })
//                .FirstOrDefault();

//            return Ok(mergedTask);

//        }





//        [AllowAnonymous]
//        [HttpGet("status/{empId}")]
//        public async Task<IActionResult> GetTaskStatus(string empId)
//        {
//            var filteredTasks = await _context.Tasks.Where(t => t.EmpId == empId).ToListAsync();

//            var taskStatusCounts = filteredTasks
//                .GroupBy(t => t.Status)
//                .Select(g => new
//                {
//                    Status = g.Key.ToString(), // Convert Enum to string
//                    Count = g.Count()
//                })
//                .ToList();

//            return Ok(taskStatusCounts);
//        }

//        //[AllowAnonymous]
//        //[HttpGet("{empId}")]
//        //public async Task<IActionResult> GetTasksByEmpId(string empId)
//        //{
//        //    if (string.IsNullOrEmpty(empId))
//        //    {
//        //        return BadRequest(new { message = "Employee Id is Required" });
//        //    }
//        //    //var tasks = await _context.Tasks.ToListAsync();
//        //    var tasks = await _context.Tasks.Where(t => t.EmpId!.Equals(empId)).ToListAsync();
//        //    var groupedTasks = tasks.GroupBy(t => t.Status).Select(g => new
//        //    {
//        //        Status = g.Key.ToString(),
//        //        Count = g.Count(),
//        //        Tasks = g.ToList(),
//        //    }).ToList();


//        //    return Ok(groupedTasks);
//        //}


//        //[AllowAnonymous]
//        //[HttpGet("{empId}")]
//        //public async Task<IActionResult> GetTasksByEmpId(string empId, string? search, string? dateRange, string? type)
//        //{
//        //    if (string.IsNullOrEmpty(empId))
//        //    {
//        //        return BadRequest(new { message = "Employee Id is Required" });
//        //    }

//        //    var tasks = _context.Tasks.Where(t => t.EmpId == empId);

//        //    // Search by Task Name
//        //    if (!string.IsNullOrEmpty(search))
//        //    {
//        //        tasks = tasks.Where(t => t.Task.Contains(search));
//        //    }

//        //    // Filter by Due Date (Today, This Week, This Month)
//        //    if (!string.IsNullOrEmpty(dateRange))
//        //    {
//        //        DateTime now = DateTime.UtcNow;
//        //        if (dateRange == "today")
//        //            tasks = tasks.Where(t => t.DueDate.Date == now.Date);
//        //        else if (dateRange == "week")
//        //            tasks = tasks.Where(t => t.DueDate >= now.Date && t.DueDate <= now.AddDays(7));
//        //        else if (dateRange == "month")
//        //            tasks = tasks.Where(t => t.DueDate >= now.Date && t.DueDate <= now.AddMonths(1));
//        //    }

//        //    // Filter by Billing Type
//        //    if (!string.IsNullOrEmpty(type))
//        //    {
//        //        tasks = tasks.Where(t => t.BillingType == type);
//        //    }

//        //    return Ok(await tasks.ToListAsync());
//        //}


//        [AllowAnonymous]
//        [HttpGet("{empId}")]
//        public async Task<IActionResult> GetTasksByEmpId(string empId)
//        {
//            if (string.IsNullOrEmpty(empId))
//            {
//                return BadRequest(new { message = "Employee Id is Required" });
//            }
//            //var tasks = await _context.Tasks.ToListAsync();
//            var tasks = await _context.Tasks.Where(t => t.EmpId!.Equals(empId)).ToListAsync();
//            return Ok(tasks);
//        }



//        [AllowAnonymous]
//        [HttpPut("{Id}")]
//        public async Task<IActionResult> UpdateTaskById(int Id, [FromBody] TaskModel updatedTask)
//        {
//            if (Id != updatedTask.Id)
//                return BadRequest("Task ID mismatch");

//            var existingTask = await _context.Tasks.FindAsync(Id);
//            if (existingTask == null)
//                return NotFound();

//            existingTask.Status = updatedTask.Status;

//            await _context.SaveChangesAsync();
//            return Ok(existingTask);
//        }


//        [AllowAnonymous]
//        [HttpGet("{taskId}/timesheets")]
//        public async Task<IActionResult> GetTimesheetsByTaskId(int taskId)
//        {
//            var timesheets = await _context.Timesheets
//                                           .Where(t => t.TaskId == taskId)
//                                           .ToListAsync();



//            if (timesheets == null || !timesheets.Any())
//            {
//                return NotFound(new { message = "No timesheets found for this task!" });
//            }

//            return Ok(timesheets);
//        }


//        [AllowAnonymous]
//        [HttpPut("update-task/{taskID}")]
//        public async Task<IActionResult> UpdateTaskAssignees(string taskID, [FromBody] UpdateTaskRequest request)
//        {
//            var taskRecords = await _context.Tasks
//                .Where(t => t.TaskID == taskID)
//                .ToListAsync();

//            if (!taskRecords.Any()) return NotFound("Task not found.");

//            var existingAssignees = taskRecords.Select(t => t.EmpId).ToHashSet();

//            // Find new assignees (not already in existing tasks)
//            var newAssignees = request.Assignees!
//                .Where(a => !existingAssignees.Contains(a.EmpId))
//                .ToList();

//            foreach (var assignee in newAssignees)
//            {
//                if (!string.IsNullOrEmpty(assignee.EmpId))
//                {
//                    _context.Tasks.Add(new TaskModel
//                    {
//                        TaskID = taskID,
//                        Task = taskRecords.First().Task,
//                        Assignee = assignee.AssigneeName,
//                        EmpId = assignee.EmpId,
//                        StartDate = taskRecords.First().StartDate,
//                        DueDate = taskRecords.First().DueDate,
//                        BillingType = taskRecords.First().BillingType,
//                        Status = 0
//                    });
//                }
//            }

//            // Find assignees to remove (not in request)
//            var assigneesToRemove = taskRecords
//                .Where(t => !request.Assignees!.Any(a => a.EmpId == t.EmpId))
//                .ToList();

//            _context.Tasks.RemoveRange(assigneesToRemove);

//            await _context.SaveChangesAsync();
//            return Ok(new { message = "Task Updated Successfully" });
//        }










//        [HttpPut("update-task")]
//        public async Task<IActionResult> UpdateTotalTask([FromBody] List<TaskUpdateDto> updatedTasks)
//        {
//            if (updatedTasks == null || updatedTasks.Count == 0)
//                return BadRequest("Invalid task data");

//            string taskId = updatedTasks.First().TaskID;

//            // Get existing task records for this TaskID
//            var existingTasks = await _context.Tasks
//                .Where(t => t.TaskID == taskId)
//                .ToListAsync();

//            // Convert existing tasks to a dictionary for quick lookup
//            //var existingTaskMap = existingTasks.ToDictionary(t => t.EmpId, t => t);

//            var existingTaskMap = existingTasks
//               .Where(t => !string.IsNullOrEmpty(t.EmpId)) // Avoid null or empty keys
//               .GroupBy(t => t.EmpId)  // Ensure uniqueness
//               .ToDictionary(g => g.Key, g => g.First());



//            foreach (var updatedTask in updatedTasks)
//            {
//                if (existingTaskMap.ContainsKey(updatedTask.EmpId))
//                {
//                    // Update existing task details
//                    var taskToUpdate = existingTaskMap[updatedTask.EmpId];
//                    taskToUpdate.Task = updatedTask.Task;
//                    taskToUpdate.BillingType = updatedTask.BillingType;
//                    taskToUpdate.StartDate = updatedTask.StartDate;
//                    taskToUpdate.DueDate = updatedTask.DueDate;

//                    _context.Tasks.Update(taskToUpdate);
//                    existingTaskMap.Remove(updatedTask.EmpId); // Mark as processed
//                }
//                else
//                {
//                    // Insert new assignee
//                    _context.Tasks.Add(new TaskModel
//                    {
//                        TaskID = updatedTask.TaskID,
//                        Task = updatedTask.Task,
//                        BillingType = updatedTask.BillingType,
//                        StartDate = updatedTask.StartDate,
//                        DueDate = updatedTask.DueDate,
//                        Assignee = updatedTask.Assignee,
//                        EmpId = updatedTask.EmpId
//                    });
//                }
//            }

//            // Remove assignees that were not in the updated list
//            _context.Tasks.RemoveRange(existingTaskMap.Values);

//            await _context.SaveChangesAsync();
//            //return Ok("Task updated successfully");
//            return Ok(new { message = "Task Updated Successfully" });

//        }











//        [AllowAnonymous]
//        [HttpDelete("delete-task/{taskId}")]
//        public async Task<IActionResult> DeleteTask(string taskId)
//        {
//            //var taskRecords = await _context.Tasks.Where(t => t.TaskID == taskID).ToListAsync();
//            var taskRecords = await _context.Tasks.Where(t => t.TaskID == taskId).ToListAsync();
//            foreach (var task in taskRecords)
//            {
//                _context.Tasks.Remove(task);
//            }


//            await _context.SaveChangesAsync();
//            //return Ok(taskRecords);
//            return Ok(new { message = "Task Deleted Successfully" });
//        }






//    }




//}





using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeSnapBackend_MySql.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using TimeSnapBackend_MySql.Dtos;

namespace TimeSnapBackend_MySql.Controllers
{
    [Route("api")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskController(AppDbContext context)
        {
            _context = context;
        }



        [AllowAnonymous]
        [HttpPost("tasks/upload")]
        public async Task<IActionResult> UploadTasks([FromBody] List<TaskUploadDto> taskDtos)
        {
            if (taskDtos == null || taskDtos.Count == 0)
            {
                return BadRequest(new { message = "Invalid task data" });
            }

            foreach (var dto in taskDtos)
            {
                // Check if task already exists based on TaskId
                var task = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskId == dto.TaskID);

                if (task == null)
                {
                    // Create new Task
                    task = new TaskModel
                    {
                        TaskName = dto.Task,
                        TaskId = dto.TaskID,
                        StartDate = dto.StartDate,
                        DueDate = dto.DueDate,
                        BillingType = dto.BillingType
                    };
                    _context.Tasks.Add(task);
                    await _context.SaveChangesAsync();
                }

                // Check if the assignee already exists in UserTasks
                var existingUserTask = await _context.UserTasks.FirstOrDefaultAsync(ut => ut.TaskId == task.TaskId && ut.EmpId == dto.EmpId);
                if (existingUserTask == null)
                {
                    var userTask = new UserTask
                    {
                        TaskId = task.TaskId,
                        EmpId = dto.EmpId,
                        Status = Models.TasksStatus.NotStarted // Default status
                    };
                    _context.UserTasks.Add(userTask);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Tasks uploaded successfully" });
        }





        //[AllowAnonymous]
        [HttpGet("tasks")]
        public async Task<IActionResult> GetTasks([FromQuery] string? billingType)
        {
            var tasksQuery = _context.Tasks
                .Include(t => t.UserTasks) // Include UserTasks
                .ThenInclude(ut => ut.Employee) // Include Employee details
                .AsQueryable();

            if (!string.IsNullOrEmpty(billingType))
            {
                tasksQuery = tasksQuery.Where(t => t.BillingType == billingType);
            }

            var tasks = await tasksQuery.ToListAsync();

            var result = tasks.Select(task => new
            {
                task.TaskId,
                task.TaskName,
                task.StartDate,
                task.DueDate,
                task.BillingType,
                Assignees = task.UserTasks.Select(ut => new
                {
                    ut.EmpId,
                    ut.Employee.FullName // Assuming UserName exists in AppUser
                }).ToList()
            });

            return Ok(result);
        }




        //[AllowAnonymous]
        [HttpGet("tasks/{empId}")]
        public async Task<IActionResult> GetUserTasks(string empId)
        {
            var userTasks = await _context.UserTasks
                .Where(ut => ut.EmpId == empId) // Filter by specific user
                .Join(_context.Tasks,
                      ut => ut.TaskId,
                      t => t.TaskId,
                      (ut, t) => new
                      {
                          t.Id,
                          t.TaskId,
                          t.TaskName,
                          t.StartDate,
                          t.DueDate,
                          t.BillingType,
                          ut.Status,
                          ut.CompletedDate
                      })
                .ToListAsync();

            return Ok(userTasks);
        }






        [AllowAnonymous]
        [HttpGet("task/{taskId}")]
        public async Task<IActionResult> GetTaskById(string taskId)
        {
            var task = await _context.Tasks
                .Where(t => t.TaskId == taskId)
                .Select(t => new
                {
                    t.Id,
                    t.TaskId,
                    t.TaskName,
                    t.StartDate,
                    t.DueDate,
                    t.BillingType
                })
                .FirstOrDefaultAsync();

            if (task == null)
            {
                return NotFound(new { message = "Task not found" });
            }

            return Ok(task);
        }






        [AllowAnonymous]
        [HttpGet("task-details/{taskId}")]
        public async Task<IActionResult> GetTaskDetails(string taskId)
        {
            var taskDetails = await (from t in _context.Tasks
                                     where t.TaskId == taskId
                                     select new TaskDetailsDto
                                     {
                                         TaskId = t.TaskId,
                                         TaskName = t.TaskName,
                                         StartDate = t.StartDate,
                                         DueDate = t.DueDate,
                                         BillingType = t.BillingType,
                                         AssignedEmployees = (from ut in _context.UserTasks
                                                              join ue in _context.UserEmployees on ut.EmpId equals ue.EmployeeId
                                                              where ut.TaskId == taskId
                                                              select new EmployeeDto
                                                              {
                                                                  EmpId = ue.EmployeeId,
                                                                  Assignee = ue.UserName
                                                              }).ToList()
                                     }).FirstOrDefaultAsync();

            if (taskDetails == null)
                return NotFound(new { message = "Task not found" });

            return Ok(taskDetails);
        }















        [HttpPost("tasks/add-task")]
        public async Task<IActionResult> AddTask([FromBody] TaskCreateDto taskDto)
        {
            if (taskDto == null || taskDto.Assignee == null || !taskDto.Assignee.Any())
            {
                return BadRequest(new { message = "Invalid task data" });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1️ Add Task to 'tasks' Table
                var task = new TaskModel
                {
                    TaskId = taskDto.TaskId,
                    TaskName = taskDto.TaskName,
                    StartDate = taskDto.StartDate,
                    DueDate = taskDto.DueDate,
                    BillingType = taskDto.BillingType
                };

                await _context.Tasks.AddAsync(task);
                await _context.SaveChangesAsync();

                // 2️ Add Each Assignee to 'usertasks' Table
                var userTasks = taskDto.Assignee.Select(a => new UserTask
                {
                    TaskId = taskDto.TaskId,
                    EmpId = a.EmpId,
                    Status = 0, // Default status
                    CompletedDate = null // Not completed yet
                }).ToList();

                await _context.UserTasks.AddRangeAsync(userTasks);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return Ok(new { message = "Task added successfully" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Error adding task", error = ex.Message });
            }
        }






        [HttpPut("tasks/update-task")]
        public async Task<IActionResult> UpdateTask([FromBody] TaskCreateDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.TaskId))
            {
                return BadRequest("Invalid request data.");
            }

            // Find the task in the database
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskId == request.TaskId);
            if (task == null)
            {
                return NotFound("Task not found.");
            }

            // Update task details (except TaskId)
            task.TaskName = request.TaskName;
            task.DueDate = request.DueDate;
            task.StartDate = request.StartDate;
            task.BillingType = request.BillingType;
            _context.Tasks.Update(task);

            // Get existing assignees for the task
            var existingAssignees = await _context.UserTasks
                .Where(ut => ut.TaskId == request.TaskId)
                .ToListAsync();

            var existingEmpIds = existingAssignees.Select(ut => ut.EmpId).ToList();
            var updatedEmpIds = request.Assignee.Select(a => a.EmpId).ToList();

            // Remove assignees who are no longer in the updated request
            var assigneesToRemove = existingAssignees.Where(ut => !updatedEmpIds.Contains(ut.EmpId)).ToList();
            _context.UserTasks.RemoveRange(assigneesToRemove);

            // Add new assignees if not already assigned
            foreach (var assignee in request.Assignee)
            {
                if (!existingEmpIds.Contains(assignee.EmpId))
                {
                    var newAssignment = new UserTask
                    {
                        TaskId = request.TaskId,
                        EmpId = assignee.EmpId,
                        Status = 0, // Default status (Not Started)
                        CompletedDate = null
                    };
                    _context.UserTasks.Add(newAssignment);
                }
            }

            // Save changes
            await _context.SaveChangesAsync();

            return Ok(new { message = "Task updated successfully" });
        }





        [AllowAnonymous]
        [HttpPut("tasks/update-task/status")]
        public async Task<IActionResult> UpdateTaskStatus(
    [FromQuery] string taskId,
    [FromQuery] string empId,
    [FromBody] UpdateTaskDto updateTaskDto)
        {
            if (updateTaskDto.Status == TasksStatus.Completed && !updateTaskDto.CompletedDate.HasValue)
            {
                return BadRequest("Completed date is required when status is Completed.");
            }

            var task = await _context.UserTasks
                .FirstOrDefaultAsync(t => t.TaskId == taskId && t.EmpId == empId);

            if (task == null)
            {
                return NotFound("Task not found.");
            }

            task.Status = updateTaskDto.Status;

            if (updateTaskDto.Status == TasksStatus.Completed)
            {
                task.CompletedDate = updateTaskDto.CompletedDate;
            }
            else
            {
                task.CompletedDate = null; // Reset if not completed
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Task updated successfully." });

        }






        [HttpPut("tasks/update-task-assignees/{taskId}")]
        public async Task<IActionResult> UpdateTaskAssignees(string taskId, [FromBody] List<AssigneeDto> updatedAssignees)
        {
            if (string.IsNullOrEmpty(taskId) || updatedAssignees == null || updatedAssignees.Count == 0)
            {
                return BadRequest("Invalid request data.");
            }

            // Fetch existing assignees for the given taskId
            var existingAssignees = await _context.UserTasks
                .Where(ut => ut.TaskId == taskId)
                .ToListAsync();

            var existingEmpIds = existingAssignees.Select(ut => ut.EmpId).ToList();
            var updatedEmpIds = updatedAssignees.Select(a => a.EmpId).ToList();

            // Remove assignees who are no longer in the updated list
            var assigneesToRemove = existingAssignees.Where(ut => !updatedEmpIds.Contains(ut.EmpId)).ToList();
            _context.UserTasks.RemoveRange(assigneesToRemove);

            // Add new assignees who are not already in the task
            foreach (var assignee in updatedAssignees)
            {
                if (!existingEmpIds.Contains(assignee.EmpId))
                {
                    var newAssignment = new UserTask
                    {
                        TaskId = taskId,
                        EmpId = assignee.EmpId,
                        Status = 0, // Default status (Not Started)
                        CompletedDate = null
                    };
                    _context.UserTasks.Add(newAssignment);
                }
            }

            // Save changes
            await _context.SaveChangesAsync();

            return Ok(new { message = "Task assignees updated successfully." });

        }






        [HttpDelete("tasks/delete-task/{taskId}")]
        public async Task<IActionResult> DeleteTask(string taskId)
        {
            if (string.IsNullOrEmpty(taskId))
            {
                return BadRequest("Task ID is required.");
            }

            // Find the task in the database
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskId == taskId);
            if (task == null)
            {
                return NotFound("Task not found.");
            }

            // Remove related entries from UserTasks
            //var userTasks = _context.UserTasks.Where(ut => ut.TaskId == taskId);
            //_context.UserTasks.RemoveRange(userTasks);

            // Remove the task
            _context.Tasks.Remove(task);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok(new { message = "Task deleted successfully" });
        }




    }
}

