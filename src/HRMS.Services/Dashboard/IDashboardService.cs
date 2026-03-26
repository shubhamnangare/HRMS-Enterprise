using HRMS.Services.Dashboard.Dtos;

namespace HRMS.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardDataAsync();
        Task<object> GetStatisticsAsync();
        Task<ChartDataDto> GetEmployeeChartDataAsync();
        Task<List<RecentActivityDto>> GetRecentActivitiesAsync();
        Task<IEnumerable<UpcomingLeaveDto>> GetUpcomingLeavesAsync();
        Task<object> GetDepartmentDistributionAsync();
    }
}