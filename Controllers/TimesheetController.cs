//using TimeSnapBackend_MySql.Dtos;
//using TimeSnapBackend_MySql.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Threading.Tasks;

//namespace TimeSnapBackend_MySql.Controllers
//{
//    [Route("api/timesheet")]
//    [ApiController]
//    public class TimesheetController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public TimesheetController(AppDbContext context)
//        {
//            _context = context;
//        }

//        [AllowAnonymous]
//        [HttpPost]
//        public async Task<ActionResult<Timesheet>> CreateTimesheet([FromBody] TimesheetDto timesheetDto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            // Retrieve the task and employee (AppUser) using the provided taskId and empId
//            var task = await _context.Tasks.FindAsync(timesheetDto.TaskId);
//            var employee = await _context.AppUsers.FirstOrDefaultAsync(u => u.EmpId == timesheetDto.EmpId);

//            if (task == null)
//                return NotFound("Task not found");
//            if (employee == null)
//                return NotFound("Employee not found");

//            // Create and save the timesheet entry
//            var timesheet = new Timesheet
//            {
//                EmpId = timesheetDto.EmpId,
//                TaskId = timesheetDto.TaskId,
//                Date = DateTime.SpecifyKind(timesheetDto.Date ?? DateTime.Now, DateTimeKind.Local),
//                //EndTime = timesheetDto.EndTime,
//                TotalMinutes = timesheetDto.TotalMinutes
//            };

//            _context.Timesheets.Add(timesheet);
//            await _context.SaveChangesAsync();

//            return CreatedAtAction(nameof(CreateTimesheet), new { id = timesheet.Id }, timesheet);
//        }


//        [AllowAnonymous]
//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdateEndTime(int id, [FromBody] EndTimeRequest request)
//        {
//            var timesheet = await _context.Timesheets.FindAsync(id);
//            if (timesheet == null)
//            {
//                return NotFound();
//            }

//            // Update the endTime
//            //timesheet.EndTime = request.EndTime;
//            timesheet.Date = request.Date;
//            timesheet.TotalMinutes = request.TotalMinutes;


//            // Save changes to the database
//            await _context.SaveChangesAsync();

//            //return NoContent();
//            return Ok(new { message = "Timesheet Updated successfully" }); // ✅ Return data

//        }


//        [AllowAnonymous]
//        [HttpPost("addlog")]
//        public async Task<ActionResult<Timesheet>> AddTimeLog([FromBody] TimesheetDto timesheetDto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            // Retrieve the task and employee (AppUser) using the provided taskId and empId
//            var task = await _context.Tasks.FindAsync(timesheetDto.TaskId);
//            var employee = await _context.AppUsers.FirstOrDefaultAsync(u => u.EmpId == timesheetDto.EmpId);

//            if (task == null)
//                return NotFound("Task not found");
//            if (employee == null)
//                return NotFound("Employee not found");

//            // Create and save the timesheet entry
//            var timesheet = new Timesheet
//            {
//                EmpId = timesheetDto.EmpId,
//                TaskId = timesheetDto.TaskId,
//                Date = timesheetDto.Date!.Value,
//                //EndTime = timesheetDto.EndTime.Value,
//                TotalMinutes = timesheetDto.TotalMinutes,
//                Notes = timesheetDto.Notes,
//            };

//            _context.Timesheets.Add(timesheet);
//            await _context.SaveChangesAsync();

//            return CreatedAtAction(nameof(CreateTimesheet), new { id = timesheet.Id }, timesheet);
//        }




//        [Authorize(Roles = "Admin")]
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<object>>> GetTimesheets([FromQuery] string? billingType)
//        {
//            var timesheetsQuery = _context.Timesheets
//                .Join(_context.Tasks,
//                      ti => ti.TaskId,
//                      ta => ta.Id,
//                      (ti, ta) => new
//                      {
//                          ti.Id,
//                          ti.EmpId,
//                          ti.TaskId,
//                          ti.Date,
//                          ti.TotalMinutes,
//                          ti.Notes,
//                          UserName = ta.Assignee,
//                          TaskName = ta.Task,
//                          ta.BillingType,
//                      });

//            // Apply billingType filter if provided
//            if (!string.IsNullOrEmpty(billingType))
//            {
//                timesheetsQuery = timesheetsQuery.Where(t => t.BillingType == billingType);
//            }

//            var timesheets = await timesheetsQuery.ToListAsync();

//            if (!timesheets.Any())
//            {
//                return NotFound(new { message = "No timesheets found!" });
//            }

//            return Ok(timesheets);
//        }









//        [AllowAnonymous]
//        [HttpDelete("{timeSheetId}")]
//        public async Task<IActionResult> DeleteTimeSheet(int timeSheetId)
//        {
//            //var taskRecords = await _context.Tasks.Where(t => t.TaskID == taskID).ToListAsync();
//            var timeSheet = await _context.Timesheets.Where(t => t.Id == timeSheetId).ToListAsync();
//            if (!timeSheet.Any())
//            {
//                return NotFound("Time Sheet Not Found");
//            }
//            else
//            {
//                _context.Timesheets.Remove(timeSheet[0]);
//            }

//            await _context.SaveChangesAsync();
//            //return Ok(taskRecords);
//            return Ok(new { message = "Time Sheet Deleted Successfully" });
//        }



//    }
//}






using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeSnapBackend_MySql.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using TimeSnapBackend_MySql.Dtos;

namespace TimeSnapBackend_MySql.Controllers
{
    [Route("api/timesheet")]
    [ApiController]
    public class TimesheetController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TimesheetController(AppDbContext context)
        {
            _context = context;
        }

        //[AllowAnonymous]
        //[HttpPost]
        //public async Task<ActionResult<Timesheet>> CreateTimesheet([FromBody] TimesheetDto timesheetDto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var task = await _context.Tasks.FindAsync(timesheetDto.TaskId);
        //    var employee = await _context.AppUsers.FirstOrDefaultAsync(u => u.EmpId == timesheetDto.EmpId);

        //    if (task == null)
        //        return NotFound("Task not found");
        //    if (employee == null)
        //        return NotFound("Employee not found");

        //    var timesheet = new Timesheet
        //    {
        //        EmpId = timesheetDto.EmpId,
        //        TaskId = timesheetDto.TaskId,
        //        Date = timesheetDto.Date ?? DateTime.Now,
        //        TotalMinutes = timesheetDto.TotalMinutes
        //    };

        //    _context.Timesheets.Add(timesheet);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(CreateTimesheet), new { id = timesheet.Id }, timesheet);
        //}


        //[AllowAnonymous]
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateEndTime(int id, [FromBody] EndTimeRequest request)
        //{
        //    var timesheet = await _context.Timesheets.FindAsync(id);
        //    if (timesheet == null)
        //        return NotFound();

        //    timesheet.Date = request.Date;
        //    timesheet.TotalMinutes = request.TotalMinutes;

        //    await _context.SaveChangesAsync();

        //    return Ok(new { message = "Timesheet updated successfully" });
        //}

        //[Authorize(Roles = "Admin")]
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<object>>> GetTimesheets([FromQuery] string? billingType)
        //{
        //    var timesheetsQuery = _context.Timesheets
        //        .Join(_context.Tasks,
        //              ts => ts.TaskId,
        //              ta => ta.Id,
        //              (ts, ta) => new
        //              {
        //                  ts.Id,
        //                  ts.EmpId,
        //                  ts.TaskId,
        //                  ts.Date,
        //                  ts.TotalMinutes,
        //                  ts.Notes,
        //                  TaskName = ta.TaskName,
        //                  ta.BillingType
        //              })
        //        .Join(_context.AppUsers,
        //              ts => ts.EmpId,
        //              user => user.EmpId,
        //              (ts, user) => new
        //              {
        //                  ts.Id,
        //                  ts.EmpId,
        //                  ts.TaskId,
        //                  ts.Date,
        //                  ts.TotalMinutes,
        //                  ts.Notes,
        //                  ts.TaskName,
        //                  ts.BillingType,
        //                  EmployeeName = user.FullName
        //              });

        //    if (!string.IsNullOrEmpty(billingType))
        //        timesheetsQuery = timesheetsQuery.Where(t => t.BillingType == billingType);

        //    var timesheets = await timesheetsQuery.ToListAsync();

        //    if (!timesheets.Any())
        //        return NotFound(new { message = "No timesheets found!" });

        //    return Ok(timesheets);
        //}




        //[AllowAnonymous]
        //[HttpPost("addlog")]
        //public async Task<ActionResult<Timesheet>> AddTimeLog([FromBody] TimesheetDto timesheetDto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    // Retrieve the task and employee (AppUser) using the provided taskId and empId
        //    var task = await _context.Tasks.FindAsync(timesheetDto.TaskId);
        //    var employee = await _context.AppUsers.FirstOrDefaultAsync(u => u.EmpId == timesheetDto.EmpId);

        //    if (task == null)
        //        return NotFound("Task not found");
        //    if (employee == null)
        //        return NotFound("Employee not found");

        //    // Create and save the timesheet entry
        //    var timesheet = new Timesheet
        //    {
        //        EmpId = timesheetDto.EmpId,
        //        TaskId = timesheetDto.TaskId,
        //        Date = timesheetDto.Date!.Value,
        //        //EndTime = timesheetDto.EndTime.Value,
        //        TotalMinutes = timesheetDto.TotalMinutes,
        //        Notes = timesheetDto.Notes,
        //    };

        //    _context.Timesheets.Add(timesheet);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(CreateTimesheet), new { id = timesheet.Id }, timesheet);
        //}



        [AllowAnonymous]
        [HttpPost("addlog")]
        public async Task<ActionResult<Timesheet>> AddTimeLog([FromBody] TimesheetDto timesheetDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Retrieve the task and employee using TaskId (string) and EmpId
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskId == timesheetDto.TaskId);
            var employee = await _context.AppUsers.FirstOrDefaultAsync(u => u.EmpId == timesheetDto.EmpId);

            if (task == null)
                return NotFound("Task not found");
            if (employee == null)
                return NotFound("Employee not found");

            // Create and save the timesheet entry
            var timesheet = new Timesheet
            {
                EmpId = timesheetDto.EmpId,
                TaskId = timesheetDto.TaskId,
                Date = timesheetDto.Date!.Value,
                TotalMinutes = timesheetDto.TotalMinutes,
                Notes = timesheetDto.Notes,
            };

            _context.Timesheets.Add(timesheet);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(AddTimeLog), new { id = timesheet.Id }, timesheet);
        }




        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Timesheet>>> GetTimesheetsByUserAndTask(
                [FromQuery] string empId, [FromQuery] string taskId)
        {
            var timesheets = await _context.Timesheets
                .Where(t => t.EmpId == empId && t.TaskId == taskId)
                .ToListAsync();

            if (!timesheets.Any())
                return NotFound("No timesheets found for the given user and task.");

            return Ok(timesheets);
        }





        //[AllowAnonymous]
        //[HttpGet("filtered-timesheets")]
        //public async Task<ActionResult> GetFilteredTimesheets(
        //    [FromQuery] string billingType, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        //{
        //    var query = from t in _context.Timesheets
        //                join u in _context.UserEmployees on t.EmpId equals u.EmployeeId
        //                join ts in _context.Tasks on t.TaskId equals ts.TaskId
        //                where t.Date >= startDate.Date && t.Date <= endDate.Date && ts.BillingType == billingType
        //                select new TimesheetResponseDto
        //                {
        //                    UserName = u.UserName,
        //                    EmpId = t.EmpId,
        //                    TaskName = ts.TaskName,
        //                    Date = t.Date,
        //                    BillingType = ts.BillingType,
        //                    TimesheetId = t.Id,
        //                    TotalMinutes = t.TotalMinutes,
        //                    Notes = t.Notes
        //                };

        //    // Group by user
        //    var groupedByUser = query
        //        .AsEnumerable() // Move grouping to in-memory processing
        //        .GroupBy(t => t.EmpId)
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .Select(g => new
        //        {
        //            EmpId = g.Key,
        //            UserName = g.First().UserName,
        //            Timesheets = g.ToList()
        //        })
        //        .ToList();

        //    int totalUsers = query.Select(t => t.EmpId).Distinct().Count();

        //    return Ok(new { timesheets = groupedByUser, totalUsers });
        //}




        [AllowAnonymous]
        [HttpGet("filtered-timesheets")]
        public async Task<ActionResult> GetFilteredTimesheets(
    [FromQuery] string billingType,
    [FromQuery] DateTime? startDate,
    [FromQuery] DateTime? endDate,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 5)
        {
            // Set default startDate (Monday) and endDate (Sunday) if not provided
            DateTime today = DateTime.UtcNow;
            DateTime currentWeekStart = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday); // Monday
            DateTime currentWeekEnd = currentWeekStart.AddDays(6); // Sunday

            DateTime filterStartDate = startDate?.Date ?? currentWeekStart;
            DateTime filterEndDate = endDate?.Date ?? currentWeekEnd;

            var query = from t in _context.Timesheets
                        join u in _context.UserEmployees on t.EmpId equals u.EmployeeId
                        join ts in _context.Tasks on t.TaskId equals ts.TaskId
                        where t.Date >= filterStartDate && t.Date <= filterEndDate && ts.BillingType == billingType
                        select new TimesheetResponseDto
                        {
                            UserName = u.UserName,
                            EmpId = t.EmpId,
                            TaskId = t.TaskId,
                            TaskName = ts.TaskName,
                            Date = t.Date,
                            BillingType = ts.BillingType,
                            TimesheetId = t.Id,
                            TotalMinutes = t.TotalMinutes,
                            Notes = t.Notes
                        };

            // Group by user
            var groupedByUser = query
                .AsEnumerable()
                .GroupBy(t => t.EmpId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(g => new
                {
                    EmpId = g.Key,
                    UserName = g.First().UserName,
                    Timesheets = g.ToList()
                })
                .ToList();

            int totalUsers = query.Select(t => t.EmpId).Distinct().Count();

            return Ok(new { timesheets = groupedByUser, totalUsers });
        }










        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTimeSheet(int id, [FromBody] UpdateTimeSheetRequest request)
        {
            var timesheet = await _context.Timesheets.FindAsync(id);
            if (timesheet == null)
            {
                return NotFound();
            }


            timesheet.Date = request.Date;
            timesheet.TotalMinutes = request.TotalMinutes;
            timesheet.Notes = request.Notes;


            // Save changes to the database
            await _context.SaveChangesAsync();

            //return NoContent();
            return Ok(new { message = "Timesheet Updated successfully" });

        }




        [AllowAnonymous]
        [HttpDelete("{timeSheetId}")]
        public async Task<IActionResult> DeleteTimeSheet(int timeSheetId)
        {
            //var taskRecords = await _context.Tasks.Where(t => t.TaskID == taskID).ToListAsync();
            var timeSheet = await _context.Timesheets.Where(t => t.Id == timeSheetId).ToListAsync();
            if (!timeSheet.Any())
            {
                return NotFound("Time Sheet Not Found");
            }
            else
            {
                _context.Timesheets.Remove(timeSheet[0]);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Time Sheet Deleted Successfully" });
        }


    }
}
