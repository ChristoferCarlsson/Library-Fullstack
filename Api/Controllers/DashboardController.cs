using Application.DTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        // GET: api/dashboard
        [HttpGet]
        public async Task<ActionResult<DashboardDto>> GetDashboard()
        {
            var data = await _dashboardService.GetDashboardAsync();
            return Ok(data);
        }
    }
}
