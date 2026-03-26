using Microsoft.AspNetCore.Identity;

namespace HRMS.Web.ViewModels;

public class UserRoleViewModel
{
    public IdentityUser? User { get; set; }
    public IList<string>? Roles { get; set; }
    public List<RoleSelection>? AvailableRoles { get; set; }
}

public class RoleSelection
{
    public string? RoleName { get; set; }
    public bool IsSelected { get; set; }
}