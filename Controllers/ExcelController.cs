using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TimeSnapBackend_MySql.Services;

namespace TimeSnapBackend_MySql.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        private readonly ExcelService _excelService;

        public ExcelController()
        {
            _excelService = new ExcelService();
        }

        [AllowAnonymous]
        [HttpPost("upload")]
        [Consumes("multipart/form-data")] // Tell Swagger that the request contains a file
        [SwaggerOperation(Summary = "Upload an Excel file", Description = "Uploads an Excel file and returns filtered employees")]
        public IActionResult Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            using var stream = file.OpenReadStream();
            _excelService.ReadExcelFile(stream);

            var filteredEmployees = _excelService.GetFilteredEmployees();
            var excelFile = _excelService.GenerateExcel(filteredEmployees);

            return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FilteredEmployees.xlsx");
        }



    }

}
