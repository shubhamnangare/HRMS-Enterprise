using HRMS.Services.Dashboard.Dtos;

namespace HRMS.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardDataAsync();
        Task<object> GetStatisticsAsync();
        Task<ChartDataDto> GetEmployeeChartDataAsync();
        Task<List<RecentActivityDto>> GetRecentActivitiesAsync();
        Task<object> GetUpcomingLeavesAsync();
        Task<object> GetDepartmentDistributionAsync();
    }
}