using HRMS.Core.Entities;
using HRMS.Core.Entities.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security.Claims;

namespace HRMS.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    private readonly IHttpContextAccessor? _httpContextAccessor;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor? httpContextAccessor = null)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // DbSets
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<LeaveRequest> LeaveRequests { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
   // public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all configurations from this assembly
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Global query filters
        builder.Entity<Employee>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Department>().HasQueryFilter(d => !d.IsDeleted);
        builder.Entity<LeaveRequest>().HasQueryFilter(l => !l.IsDeleted);
        builder.Entity<Attendance>().HasQueryFilter(a => !a.IsDeleted);
        builder.Entity<AuditLog>().HasQueryFilter(a => !a.IsDeleted);

        // Configure relationships
        ConfigureRelationships(builder);

        // Configure indexes
        ConfigureIndexes(builder);

        // Seed initial data
        SeedData(builder);

        // Configure Identity tables if needed
        ConfigureIdentityTables(builder);
    }

    private void ConfigureRelationships(ModelBuilder builder)
    {
        // Employee - Department relationship
        builder.Entity<Employee>()
            .HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Employee - Manager relationship (self-referencing)
        builder.Entity<Employee>()
            .HasOne(e => e.Manager)
            .WithMany(e => e.Subordinates)
            .HasForeignKey(e => e.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        // LeaveRequest - Employee relationship
        builder.Entity<LeaveRequest>()
            .HasOne(l => l.Employee)
            .WithMany(e => e.LeaveRequests)
            .HasForeignKey(l => l.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // LeaveRequest - ApprovedBy relationship
        builder.Entity<LeaveRequest>()
            .HasOne(l => l.ApprovedBy)
            .WithMany()
            .HasForeignKey(l => l.ApprovedById)
            .OnDelete(DeleteBehavior.Restrict);

        // Attendance - Employee relationship
        builder.Entity<Attendance>()
            .HasOne(a => a.Employee)
            .WithMany(e => e.Attendances)
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private void ConfigureIndexes(ModelBuilder builder)
    {
        // Employee indexes
        builder.Entity<Employee>()
            .HasIndex(e => e.EmployeeCode)
            .IsUnique()
            .HasDatabaseName("IX_Employees_EmployeeCode");

        builder.Entity<Employee>()
            .HasIndex(e => e.Email)
            .IsUnique()
            .HasDatabaseName("IX_Employees_Email");

        builder.Entity<Employee>()
            .HasIndex(e => new { e.FirstName, e.LastName })
            .HasDatabaseName("IX_Employees_Name");

        builder.Entity<Employee>()
            .HasIndex(e => e.DepartmentId)
            .HasDatabaseName("IX_Employees_DepartmentId");

        builder.Entity<Employee>()
            .HasIndex(e => e.ManagerId)
            .HasDatabaseName("IX_Employees_ManagerId");

        builder.Entity<Employee>()
            .HasIndex(e => e.Status)
            .HasDatabaseName("IX_Employees_Status");

        builder.Entity<Employee>()
            .HasIndex(e => e.EmploymentType)
            .HasDatabaseName("IX_Employees_EmploymentType");

        // Department indexes
        builder.Entity<Department>()
            .HasIndex(d => d.Code)
            .IsUnique()
            .HasDatabaseName("IX_Departments_Code");

        builder.Entity<Department>()
            .HasIndex(d => d.Name)
            .IsUnique()
            .HasDatabaseName("IX_Departments_Name");

        builder.Entity<Department>()
            .HasIndex(d => d.ManagerId)
            .HasDatabaseName("IX_Departments_ManagerId");

        // LeaveRequest indexes
        builder.Entity<LeaveRequest>()
            .HasIndex(l => l.EmployeeId)
            .HasDatabaseName("IX_LeaveRequests_EmployeeId");

        builder.Entity<LeaveRequest>()
            .HasIndex(l => l.Status)
            .HasDatabaseName("IX_LeaveRequests_Status");

        builder.Entity<LeaveRequest>()
            .HasIndex(l => new { l.StartDate, l.EndDate })
            .HasDatabaseName("IX_LeaveRequests_DateRange");

        builder.Entity<LeaveRequest>()
            .HasIndex(l => l.LeaveType)
            .HasDatabaseName("IX_LeaveRequests_LeaveType");


        // Attendance indexes
        builder.Entity<Attendance>()
            .HasIndex(a => a.EmployeeId)
            .HasDatabaseName("IX_Attendances_EmployeeId");

        builder.Entity<Attendance>()
            .HasIndex(a => a.Date)
            .HasDatabaseName("IX_Attendances_Date");

        builder.Entity<Attendance>()
            .HasIndex(a => new { a.EmployeeId, a.Date })
            .IsUnique()
            .HasDatabaseName("IX_Attendances_EmployeeId_Date");

        builder.Entity<Attendance>()
            .HasIndex(a => a.Status)
            .HasDatabaseName("IX_Attendances_Status");

        // AuditLog indexes
        builder.Entity<AuditLog>()
            .HasIndex(a => a.UserId)
            .HasDatabaseName("IX_AuditLogs_UserId");

        builder.Entity<AuditLog>()
            .HasIndex(a => a.Entity)
            .HasDatabaseName("IX_AuditLogs_Entity");

        builder.Entity<AuditLog>()
            .HasIndex(a => a.EntityId)
            .HasDatabaseName("IX_AuditLogs_EntityId");

        builder.Entity<AuditLog>()
            .HasIndex(a => a.Action)
            .HasDatabaseName("IX_AuditLogs_Action");

        builder.Entity<AuditLog>()
            .HasIndex(a => a.Timestamp)
            .HasDatabaseName("IX_AuditLogs_Timestamp");
    }

    private void SeedData(ModelBuilder builder)
    {
        // Seed default admin user (will be handled by DbInitializer)
        // This is just for reference
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        var now = DateTime.UtcNow;
        var currentUser = GetCurrentUser(); // You'll need to implement this

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = currentUser;
                    entry.Entity.IsDeleted = false;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = currentUser;
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = currentUser;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ConfigureIdentityTables(ModelBuilder builder)
    {
        // Rename Identity tables to have consistent naming
        builder.Entity<IdentityUser>(entity =>
        {
            entity.ToTable("Users");
            entity.HasIndex(u => u.Email).HasDatabaseName("IX_Users_Email");
            entity.HasIndex(u => u.NormalizedEmail).HasDatabaseName("IX_Users_NormalizedEmail");
        });

        builder.Entity<IdentityRole>(entity =>
        {
            entity.ToTable("Roles");
        });

        builder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable("UserRoles");
        });

        builder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("UserClaims");
        });

        builder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("UserLogins");
        });

        builder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.ToTable("RoleClaims");
        });

        builder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("UserTokens");
        });
    }

    private string? GetCurrentUser()
    {
        // This will be implemented later with HttpContextAccessor
        if (_httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = _httpContextAccessor.HttpContext.User.Identity.Name;

            // Return user identifier (email or username)
            return !string.IsNullOrEmpty(userName) ? userName : userId;
        }

        return "System";
    }
}