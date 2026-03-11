using HRMS.Services.Departments.Dtos;
using HRMS.Services.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IDepartmentService = HRMS.Services.Departments.IDepartmentService;

namespace HRMS.Web.Controllers
{
   // [Authorize(Roles = "Admin,HR")]
    public class DepartmentController : BaseController
    {
        private readonly IDepartmentService _departmentService;
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<DepartmentController> _logger;

        public DepartmentController(
            IDepartmentService departmentService,
            IEmployeeService employeeService,
            ILogger<DepartmentController> logger)
        {
            _departmentService = departmentService;
            _employeeService = employeeService;
            _logger = logger;
        }

        // GET: Department
        public async Task<IActionResult> Index()
        {
            try
            {
                var departments = await _departmentService.GetAllDepartmentsAsync();
                return View(departments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading departments");
                return HandleException(ex, "Failed to load departments");
            }
        }

        // GET: Department/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var department = await _departmentService.GetDepartmentWithEmployeesAsync(id);
                if (department == null)
                {
                    return NotFound();
                }
                return View(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading department details for ID: {Id}", id);
                return HandleException(ex, "Failed to load department details");
            }
        }

        // GET: Department/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.Managers = await _employeeService.GetAllEmployeesAsync();
                return View(new CreateDepartmentDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create form");
                return HandleException(ex, "Failed to load create form");
            }
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDepartmentDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Managers = await _employeeService.GetAllEmployeesAsync();
                    return View(createDto);
                }

                var department = await _departmentService.CreateDepartmentAsync(createDto);
                AddSuccessMessage($"Department {department.Name} created successfully!");

                return RedirectToAction(nameof(Index));
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                ViewBag.Managers = await _employeeService.GetAllEmployeesAsync();
                return View(createDto);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("Code", ex.Message);
                ViewBag.Managers = await _employeeService.GetAllEmployeesAsync();
                return View(createDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating department");
                return HandleException(ex, "Failed to create department");
            }
        }

        // GET: Department/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var department = await _departmentService.GetDepartmentByIdAsync(id);
                if (department == null)
                {
                    return NotFound();
                }

                var updateDto = new UpdateDepartmentDto
                {
                    Id = department.Id,
                    Code = department.Code,
                    Name = department.Name,
                    Description = department.Description,
                    Location = department.Location,
                    Phone = department.Phone,
                    Email = department.Email,
                    Budget = department.Budget,
                    ManagerId = department.ManagerId
                };

                ViewBag.Managers = await _employeeService.GetAllEmployeesAsync();
                return View(updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit form for ID: {Id}", id);
                return HandleException(ex, "Failed to load edit form");
            }
        }

        // POST: Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateDepartmentDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest();
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Managers = await _employeeService.GetAllEmployeesAsync();
                    return View(updateDto);
                }

                var department = await _departmentService.UpdateDepartmentAsync(updateDto);
                AddSuccessMessage($"Department {department.Name} updated successfully!");

                return RedirectToAction(nameof(Index));
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                ViewBag.Managers = await _employeeService.GetAllEmployeesAsync();
                return View(updateDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating department ID: {Id}", id);
                return HandleException(ex, "Failed to update department");
            }
        }

        // POST: Department/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _departmentService.DeleteDepartmentAsync(id);
                if (result)
                {
                    AddSuccessMessage("Department deleted successfully!");
                }
                else
                {
                    AddErrorMessage("Cannot delete department with employees assigned to it.");
                }

                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting department ID: {Id}", id);
                return HandleException(ex, "Failed to delete department");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckCode(string code, int? id)
        {
            try
            {
                var isUnique = await _departmentService.IsDepartmentCodeUniqueAsync(code, id);
                return Json(isUnique);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking department code uniqueness");
                return Json(false);
            }
        }
    }
}