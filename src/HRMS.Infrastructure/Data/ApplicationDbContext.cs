using HRMS.Core.Entities;
using HRMS.Core.Entities.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IHttpContextAccessor? httpContextAccessor = null)
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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Global Filters (Soft Delete)
        builder.Entity<Employee>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Department>().HasQueryFilter(d => !d.IsDeleted);
        builder.Entity<LeaveRequest>().HasQueryFilter(l => !l.IsDeleted);
        builder.Entity<Attendance>().HasQueryFilter(a => !a.IsDeleted);
        builder.Entity<AuditLog>().HasQueryFilter(a => !a.IsDeleted);

        ConfigureRelationships(builder);
        ConfigureIndexes(builder);
        ConfigureDecimalPrecision(builder); // ✅ Added
        ConfigureIdentityTables(builder);
    }

    private void ConfigureRelationships(ModelBuilder builder)
    {
        builder.Entity<Employee>()
            .HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Employee>()
            .HasOne(e => e.Manager)
            .WithMany(e => e.Subordinates)
            .HasForeignKey(e => e.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<LeaveRequest>()
            .HasOne(l => l.Employee)
            .WithMany(e => e.LeaveRequests)
            .HasForeignKey(l => l.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<LeaveRequest>()
            .HasOne(l => l.ApprovedBy)
            .WithMany()
            .HasForeignKey(l => l.ApprovedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Attendance>()
            .HasOne(a => a.Employee)
            .WithMany(e => e.Attendances)
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private void ConfigureIndexes(ModelBuilder builder)
    {
        builder.Entity<Employee>().HasIndex(e => e.EmployeeCode).IsUnique();
        builder.Entity<Employee>().HasIndex(e => e.Email).IsUnique();
        builder.Entity<Employee>().HasIndex(e => new { e.FirstName, e.LastName });
        builder.Entity<Employee>().HasIndex(e => e.DepartmentId);
        builder.Entity<Employee>().HasIndex(e => e.ManagerId);

        builder.Entity<Department>().HasIndex(d => d.Code).IsUnique();
        builder.Entity<Department>().HasIndex(d => d.Name).IsUnique();

        builder.Entity<LeaveRequest>().HasIndex(l => l.EmployeeId);
        builder.Entity<LeaveRequest>().HasIndex(l => l.Status);

        builder.Entity<Attendance>()
            .HasIndex(a => new { a.EmployeeId, a.Date })
            .IsUnique();

        builder.Entity<AuditLog>().HasIndex(a => a.UserId);
    }

    // ✅ IMPORTANT FOR POSTGRESQL
    private void ConfigureDecimalPrecision(ModelBuilder builder)
    {
        builder.Entity<Employee>()
            .Property(e => e.Salary)
            .HasColumnType("numeric(18,2)");

        builder.Entity<Department>()
            .Property(d => d.Budget)
            .HasColumnType("numeric(18,2)");

        builder.Entity<Attendance>()
            .Property(a => a.TotalHours)
            .HasColumnType("numeric(5,2)");

        builder.Entity<Attendance>()
            .Property(a => a.OvertimeHours)
            .HasColumnType("numeric(5,2)");

        builder.Entity<Attendance>()
            .Property(a => a.LateMinutes)
            .HasColumnType("numeric(5,2)");
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        var now = DateTime.UtcNow;
        var currentUser = GetCurrentUser();

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
        builder.Entity<IdentityUser>().ToTable("Users");
        builder.Entity<IdentityRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
    }

    private string? GetCurrentUser()
    {
        if (_httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated == true)
        {
            return _httpContextAccessor.HttpContext.User.Identity.Name;
        }

        return "System";
    }
}