using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using TimeSnapBackend_MySql.Dtos;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TimeSnapBackend_MySql.Controllers
{
    [Route("api/reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly string _connectionString;

        public ReportsController(IConfiguration configuration)
        {
            _connectionString = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_DefaultConnection")
                                    ?? configuration.GetConnectionString("DefaultConnection");
        }

        [AllowAnonymous]
        [HttpGet("completed-tasks")]
        public async Task<IActionResult> GetCompletedTasks([FromQuery] int months)
        {
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

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var tasks = await connection.QueryAsync<TaskCompletionDto>(query, new { Months = months });

            return Ok(tasks);
        }
    }
}
