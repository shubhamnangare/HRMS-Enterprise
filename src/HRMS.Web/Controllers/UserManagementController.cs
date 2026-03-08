using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRMS.Web.ViewModels;

namespace HRMS.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<UserManagementController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserRoleViewModel
                {
                    User = user,
                    Roles = roles
                });
            }

            return View(userViewModels);
        }

        public async Task<IActionResult> ManageRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new UserRoleViewModel
            {
                User = user,
                AvailableRoles = roles.Select(r => new RoleSelection
                {
                    RoleName = r.Name,
                    IsSelected = userRoles.Contains(r.Name)
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(string id, UserRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = model.AvailableRoles?.Where(x => x.IsSelected).Select(x => x.RoleName).ToList() ?? new List<string>();

            // Remove roles that are not selected
            var rolesToRemove = userRoles.Except(selectedRoles);
            foreach (var role in rolesToRemove)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            // Add newly selected roles
            var rolesToAdd = selectedRoles.Except(userRoles);
            foreach (var role in rolesToAdd)
            {
                if (role != null)
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }

            AddSuccessMessage($"Roles updated for user {user.Email}");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MakeHR(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (!await _userManager.IsInRoleAsync(user, "HR"))
            {
                await _userManager.AddToRoleAsync(user, "HR");
                AddSuccessMessage($"User {user.Email} is now an HR member");
            }
            else
            {
                AddWarningMessage($"User {user.Email} is already in HR role");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}