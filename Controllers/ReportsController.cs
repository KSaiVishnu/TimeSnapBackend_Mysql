using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using TimeSnapBackend_MySql.Dtos;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;

namespace TimeSnapBackend_MySql.Controllers
{
    [Route("api/reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly string _connectionString;

        public ReportsController(IConfiguration configuration)
        {
            //_connectionString = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_DefaultConnection")
            //                        ?? configuration.GetConnectionString("DefaultConnection")!;

             _connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                                    ?? configuration.GetConnectionString("DefaultConnection")!;

        }

        [AllowAnonymous]
        [HttpGet("completed-tasks")]
        public async Task<IActionResult> GetCompletedTasks([FromQuery] int months)
        {
            if (months <= 0)
            {
                return BadRequest(new { message = "Months parameter must be greater than zero." });
            }


            var query = @"
            WITH UserData AS (
                SELECT 
                    ue.EmployeeId,
                    ue.UserName,
                    COUNT(ut.TaskId) AS TotalTasksCompleted,
                    ROUND(SUM(ts.TotalMinutes) / 60, 2) AS TotalHoursSpent
                FROM useremployees ue
                JOIN usertasks ut ON ue.EmployeeId = ut.EmpId
                JOIN timesheets ts ON ut.TaskId = ts.TaskId AND ut.EmpId = ts.EmpId
                WHERE ut.Status = 2 
                AND ut.CompletedDate IS NOT NULL 
                AND ut.CompletedDate != '0001-01-01'
                AND DATE(ut.CompletedDate) >= DATE_SUB(CURDATE(), INTERVAL @Months MONTH)
                GROUP BY ue.EmployeeId, ue.UserName
            )
            
            SELECT 
                ud.EmployeeId,
                ud.UserName,
                ut.TaskId,
                t.TaskName,
                t.StartDate,
                ut.CompletedDate,
                ROUND(SUM(ts.TotalMinutes) / 60, 2) AS TotalHours,
                ud.TotalTasksCompleted,
                ud.TotalHoursSpent,
                t.BillingType
            FROM UserData ud
            JOIN usertasks ut ON ud.EmployeeId = ut.EmpId
            JOIN tasks t ON ut.TaskId = t.TaskId
            LEFT JOIN timesheets ts ON ut.TaskId = ts.TaskId AND ut.EmpId = ts.EmpId
            WHERE ut.Status = 2 
            AND ut.CompletedDate IS NOT NULL 
            AND ut.CompletedDate != '0001-01-01'
            AND DATE(ut.CompletedDate) >= DATE_SUB(CURDATE(), INTERVAL @Months MONTH)
            GROUP BY ud.EmployeeId, ut.TaskId, ut.CompletedDate, t.TaskName
            ORDER BY ud.TotalHoursSpent DESC;
            ";

            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                var tasks = await connection.QueryAsync<TaskCompletionDto>(query, new { Months = months });

                if (tasks == null || !tasks.AsList().Any())
                {
                    return NotFound(new { message = "No completed tasks found for the given time period." });
                }

                return Ok(tasks);
            }

            catch (MySqlException ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Database error occurred.", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "An unexpected error occurred.", details = ex.Message });
            }

        }
    }
}
