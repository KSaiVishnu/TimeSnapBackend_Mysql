using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using TimeSnapBackend_MySql.Models;


namespace TimeSnapBackend_MySql.Controllers
{
    [Route("api/leaves")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LeaveController(AppDbContext context)
        {
            _context = context;
        }


        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            List<Timesheet> timesheets = new List<Timesheet>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Required for EPPlus

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) // Skip header row
                    {
                        object empIdCell = worksheet.Cells[row, 1].Value;
                        object leaveStartDateCell = worksheet.Cells[row, 2].Value;
                        object noOfDaysCell = worksheet.Cells[row, 3].Value;

                        if (empIdCell == null || leaveStartDateCell == null || noOfDaysCell == null)
                        {
                            Console.WriteLine($"Skipping row {row} due to missing data.");
                            continue; // Skip if any required value is missing
                        }

                        string empId = empIdCell.ToString().Trim();

                        // If the value is an email, map it to empId
                        if (empId.Contains("@"))
                        {
                            var user = await _context.UserEmployees.FirstOrDefaultAsync(u => u.Email == empId);
                            if (user == null)
                            {
                                Console.WriteLine($"Skipping row {row}: Email {empId} not found.");
                                continue; // Skip if email not found
                            }
                            empId = user.EmployeeId;
                        }

                        if (string.IsNullOrWhiteSpace(empId))
                        {
                            Console.WriteLine($"Skipping row {row}: Empty EmpId.");
                            continue;
                        }

                        // Parse LeaveStartDate (Handles both number format and string format)
                        DateTime leaveStartDate;
                        if (double.TryParse(leaveStartDateCell.ToString(), out double numericDate))
                        {
                            leaveStartDate = DateTime.FromOADate(numericDate); // Convert Excel numeric date
                        }
                        else if (DateTime.TryParse(leaveStartDateCell.ToString(), out DateTime parsedDate))
                        {
                            leaveStartDate = parsedDate;
                        }
                        else
                        {
                            Console.WriteLine($"Skipping row {row}: Invalid date format {leaveStartDateCell}.");
                            continue;
                        }

                        // Parse NoOfDays
                        if (!int.TryParse(noOfDaysCell.ToString(), out int noOfDays) || noOfDays <= 0)
                        {
                            Console.WriteLine($"Skipping row {row}: Invalid number of days {noOfDaysCell}.");
                            continue;
                        }

                        // Add leave records for each day
                        for (int i = 0; i < noOfDays; i++)
                        {
                            timesheets.Add(new Timesheet
                            {
                                EmpId = empId,
                                TaskId = "801", // Default TaskId
                                Date = leaveStartDate.AddDays(i),
                                TotalMinutes = 540, // Default TotalMinutes
                                Notes = "Leave"
                            });
                        }
                    }
                }
            }

            if (timesheets.Count > 0)
            {
                _context.Timesheets.AddRange(timesheets);
                await _context.SaveChangesAsync();
                return Ok(new { message = "File processed successfully", recordsInserted = timesheets.Count });
            }
            else
            {
                return BadRequest("No valid records found in the uploaded file.");
            }
        }
    }
}
