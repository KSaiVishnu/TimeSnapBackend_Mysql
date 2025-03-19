using TimeSnapBackend_MySql.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TimeSnapBackend_MySql.Dtos;
using Microsoft.AspNetCore.Identity;

namespace TimeSnapBackend_MySql.Controllers
{
    [Route("api/user-employee")]
    [ApiController]
    public class UserEmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserEmployeeController(AppDbContext context)
        {
            _context = context;
        }

        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<IActionResult> GetUserEmployees()
        //{
        //    var userEmployees = await _context.UserEmployees.ToListAsync();
        //    return Ok(userEmployees);

        //}


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetUserEmployees()
        {
            var usersWithRoles = from u in _context.Users
                                 join ur in _context.UserRoles on u.Id equals ur.UserId
                                 join r in _context.Roles on ur.RoleId equals r.Id
                                 select new
                                 {
                                     UserId = u.Id,
                                     UserName = u.FullName,  // Include User Name
                                     Email = u.Email,        // Include Email
                                     EmpId = u.EmpId,
                                     RoleId = r.Id,
                                     RoleName = r.Name
                                 };

            return Ok(await usersWithRoles.ToListAsync()); // Return data correctly
        }



        [AllowAnonymous]
        [HttpGet("get-last-identity")]
        public async Task<IActionResult> GetLastIdentity()
        {
            var lastIdentity = await _context.UserEmployees
                .FromSqlRaw("SELECT MAX(Id) AS Id FROM UserEmployees")
                .Select(e => e.Id)
                .FirstOrDefaultAsync();

            return Ok(new { lastIdentity });
        }



        [AllowAnonymous]
        [HttpPost("add-employee")]
        public async Task<IActionResult> AddEmployee([FromBody] List<UserEmployee> employees)
        {
            if (employees == null || employees.Count == 0)
            {
                return BadRequest(new { message = "Invalid data" });
            }

            foreach (var employee in employees)
            {
                _context.UserEmployees.Add(employee);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Employees added successfully!"});
        }


        [AllowAnonymous]
        [HttpPut("update-user-role")]
        public async Task<IActionResult> UpdateUserRole(UserRoleUpdateDto model)
        {
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == model.UserId);

            if (userRole == null)
            {
                return NotFound("User role not found.");
            }

            // Delete the old user role
            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();

            // Insert a new user role with the updated RoleId
            var newUserRole = new IdentityUserRole<string>
            {
                UserId = model.UserId,
                RoleId = model.NewRoleId  // Pass the updated RoleId from the frontend
            };

            _context.UserRoles.Add(newUserRole);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User role updated successfully." });

        }



    }
}
