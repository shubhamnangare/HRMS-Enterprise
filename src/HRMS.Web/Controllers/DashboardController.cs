using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HRMS.Services.Dashboard;
using HRMS.Services.Dashboard.Dtos;

namespace HRMS.Web.Controllers
{
    [Authorize]
    public class DashboardController : BaseController
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
                return HandleException(ex, "Failed to load dashboard data");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var stats = await _dashboardService.GetStatisticsAsync();
                return Json(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading statistics");
                return Json(new { success = false, message = "Failed to load statistics" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeChartData()
        {
            try
            {
                var chartData = await _dashboardService.GetEmployeeChartDataAsync();
                return Json(chartData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading chart data");
                return Json(new { success = false, message = "Failed to load chart data" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRecentActivities()
        {
            try
            {
                var activities = await _dashboardService.GetRecentActivitiesAsync();
                return PartialView("_RecentActivities", activities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading recent activities");
                return Content("Failed to load activities");
            }
        }
    }
}