using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace TimeSnapBackend_MySql.Services
{
    public class ExcelService
    {
        private List<EmployeeLog> _employeeLogs = new();
        private Dictionary<string, double> _leaveRecords = new(); // Store leave days
        private HashSet<string> _employeesOnLeave = new();

        public void ReadExcelFile(Stream fileStream)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(fileStream);
            var timelogSheet = package.Workbook.Worksheets["Timelog"];
            var leaveSheet = package.Workbook.Worksheets["Leaves"];

            if (timelogSheet == null || leaveSheet == null)
            {
                Console.WriteLine("Missing required sheets!");
                return;
            }

            // Read leave records first
            ReadLeaveRecords(leaveSheet);

            var logDictionary = new Dictionary<(string, DateTime), double>();

            Console.WriteLine("Reading Time Logs...");
            int rowCount = timelogSheet.Dimension?.Rows ?? 0;
            if (rowCount == 0)
            {
                Console.WriteLine("Timelog sheet is empty or unreadable!");
                return;
            }

            for (int row = 2; row <= rowCount; row++)
            {
                string user = timelogSheet.Cells[row, 1].Text.Trim();
                string dateStr = timelogSheet.Cells[row, 3].Text.Trim();
                string hoursStr = timelogSheet.Cells[row, 2].Text.Trim();

                if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(dateStr) || string.IsNullOrEmpty(hoursStr))
                {
                    Console.WriteLine($"Skipping row {row}: missing values.");
                    continue;
                }

                // Parse date
                if (!DateTime.TryParseExact(dateStr, new[] { "yyyy-MM-dd", "MM/dd/yyyy", "dd-MM-yyyy", "MM-dd-yyyy" },
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                {
                    Console.WriteLine($"Skipping row {row}: Invalid date format.");
                    continue;
                }

                // Parse hours (HH:mm)
                if (!TimeSpan.TryParseExact(hoursStr, "hh\\:mm", CultureInfo.InvariantCulture, out TimeSpan timeSpan))
                {
                    Console.WriteLine($"Skipping row {row}: Invalid hours format.");
                    continue;
                }

                double hours = timeSpan.TotalHours; // Convert HH:mm to decimal hours

                var key = (user, date);
                if (logDictionary.ContainsKey(key))
                {
                    logDictionary[key] += hours;
                }
                else
                {
                    logDictionary[key] = hours;
                }
            }

            // Convert dictionary to list before applying leave filtering
            _employeeLogs = logDictionary.Select(kv => new EmployeeLog
            {
                User = kv.Key.Item1,
                Date = kv.Key.Item2,
                HoursWorked = kv.Value
            }).ToList();

            Console.WriteLine($"Total Time Logs Read Before Filtering: {_employeeLogs.Count}");

            // Apply filtering based on leave and hours worked
            _employeeLogs = _employeeLogs.Where(log =>
            {
                bool hasHalfDayLeave = _leaveRecords.TryGetValue($"{log.User}_{log.Date:yyyy-MM-dd}", out double leaveDays) && leaveDays == 0.5;

                if (hasHalfDayLeave)
                {
                    if (log.HoursWorked >= 3 && log.HoursWorked <= 6)
                    {
                        Console.WriteLine($"Excluding {log.User} on {log.Date}: Worked {log.HoursWorked} hrs and had half-day leave.");
                        return false;
                    }
                    Console.WriteLine($"Including {log.User} on {log.Date}: Worked {log.HoursWorked} hrs but had half-day leave.");
                }

                return log.HoursWorked < 6 || hasHalfDayLeave;
            }).ToList();

            Console.WriteLine($"Total Time Logs After Filtering: {_employeeLogs.Count}");
        }

        private void ReadLeaveRecords(ExcelWorksheet leaveSheet)
        {
            Console.WriteLine("Reading Leave Records...");
            int leaveRowCount = leaveSheet.Dimension?.Rows ?? 0;

            for (int row = 2; row <= leaveRowCount; row++)
            {
                string employeeName = leaveSheet.Cells[row, 1].Text.Trim();
                string startDateStr = leaveSheet.Cells[row, 2].Text.Trim();
                string daysStr = leaveSheet.Cells[row, 3].Text.Trim();

                if (!DateTime.TryParseExact(startDateStr, new[] { "yyyy-MM-dd", "MM/dd/yyyy", "dd-MM-yyyy" },
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate) ||
                    !double.TryParse(daysStr, out double leaveDays))
                {
                    Console.WriteLine($"Skipping leave row {row}: Invalid data.");
                    continue;
                }

                for (int i = 0; i < leaveDays; i++)
                {
                    DateTime leaveDate = startDate.AddDays(i);
                    _leaveRecords[$"{employeeName}_{leaveDate:yyyy-MM-dd}"] = leaveDays;
                }
            }

            Console.WriteLine($"Total Leave Records Read: {_leaveRecords.Count}");
        }

        public List<EmployeeLog> GetFilteredEmployees()
        {
            return _employeeLogs;
        }

        public byte[] GenerateExcel(List<EmployeeLog> employees)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Filtered Employees");

            worksheet.Cells[1, 1].Value = "User";
            worksheet.Cells[1, 2].Value = "Date";
            worksheet.Cells[1, 3].Value = "Hours Worked";

            int row = 2;
            foreach (var emp in employees)
            {
                worksheet.Cells[row, 1].Value = emp.User;
                worksheet.Cells[row, 2].Value = emp.Date.ToString("yyyy-MM-dd");

                TimeSpan time = TimeSpan.FromHours(emp.HoursWorked);
                worksheet.Cells[row, 3].Value = time.ToString(@"hh\:mm");

                row++;
            }

            worksheet.Cells.AutoFitColumns();

            return package.GetAsByteArray();
        }
    }

    public class EmployeeLog
    {
        public string User { get; set; }
        public DateTime Date { get; set; }
        public double HoursWorked { get; set; }
    }
}
