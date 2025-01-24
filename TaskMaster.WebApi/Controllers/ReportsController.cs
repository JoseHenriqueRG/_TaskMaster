using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskMaster.Domain.Interfaces;

namespace TaskMaster.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IUserBusiness _userBusiness;
        private readonly IReportsBusiness _reportsBusiness;

        public ReportsController(IUserBusiness userBusiness, IReportsBusiness reportsBusiness)
        {
            _userBusiness = userBusiness;
            _reportsBusiness = reportsBusiness;
        }

        [HttpGet]
        public async Task<IActionResult> GetAverageTaskCompletionByUser(int userId)
        {
            var result = await _userBusiness.CheckUserRole(userId);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            var report = await _reportsBusiness.GeneratePerformanceReport();

            if (report.Success)
                return Ok(report);

            return StatusCode(500, report.Message);
        }
    }
}
