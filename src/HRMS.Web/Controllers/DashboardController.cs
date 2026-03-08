using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HRMS.Services.Dashboard;
using HRMS.Services.Dashboard.Dtos;

namespace HRMS.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            IDashboardService dashboardService,
            ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var dashboardData = await _dashboardService.GetDashboardDataAsync();
                return View(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard");
                TempData["Error"] = "Failed to load dashboard data";
                return View(new DashboardDto());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeChartData()
        {
            try
            {
                var data = await _dashboardService.GetEmployeeChartDataAsync();
                return Json(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading chart data");
                return Json(new { labels = new string[0], values = new int[0] });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartmentDistribution()
        {
            try
            {
                var data = await _dashboardService.GetDepartmentDistributionAsync();
                return Json(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading department distribution");
                return Json(new { labels = new string[0], values = new int[0] });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUpcomingLeaves()
        {
            try
            {
                var leaves = await _dashboardService.GetUpcomingLeavesAsync();
                return Json(leaves);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading upcoming leaves");
                return Json(new { error = "Failed to load leaves" });
            }
        }
    }
}