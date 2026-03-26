using FluentValidation;
using FluentValidation.AspNetCore;
using HRMS.Services.Dashboard;
using HRMS.Services.Departments;
using HRMS.Services.Employees;
using HRMS.Services.Leave;
using HRMS.Services.Mappings;
using HRMS.Services.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace HRMS.Services;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register all services
        services.AddAutoMapper(typeof(MappingProfile)); // Duplicate
        services.AddFluentValidationAutoValidation(); // Duplicate
        services.AddValidatorsFromAssemblyContaining<CreateEmployeeValidator>(); // Duplicate
        
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<ILeaveService, LeaveService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}
