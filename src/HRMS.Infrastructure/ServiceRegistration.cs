using HRMS.Core.Interfaces;
using HRMS.Core.Interfaces.Repositories;
using HRMS.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace HRMS.Infrastructure
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Repositories
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<ILeaveRepository, LeaveRepository>();
            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<IAuditRepository, AuditRepository>();

            return services;
        }
    }
}
