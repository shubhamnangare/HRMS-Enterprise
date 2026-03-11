using HRMS.Services.Departments;
using HRMS.Services.Employees;
using HRMS.Services.Employees.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Web.Controllers
{
   // [Authorize(Roles = "Admin,HR")]
    public class EmployeeController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(
            IEmployeeService employeeService,
            IDepartmentService departmentService,
            ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
            _logger = logger;
        }

        // GET: Employee
        public async Task<IActionResult> Index(EmployeeSearchDto searchDto)
        {
            try
            {
                var employees = await _employeeService.SearchEmployeesAsync(searchDto);
                ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
                ViewBag.SearchDto = searchDto;

                return View(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading employees");
                return HandleException(ex, "Failed to load employees");
            }
        }

        // GET: Employee/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    return NotFound();
                }
                return View(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading employee details for ID: {Id}", id);
                return HandleException(ex, "Failed to load employee details");
            }
        }

        // GET: Employee/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
                ViewBag.Managers = await _employeeService.GetAllEmployeesAsync();
                return View(new CreateEmployeeDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create form");
                return HandleException(ex, "Failed to load create form");
            }
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
                    ViewBag.Managers = await _employeeService.GetAllEmployeesAsync();
                    return View(createDto);
                }

                var employee = await _employeeService.CreateEmployeeAsync(createDto);
                AddSuccessMessage($"Employee {employee.FullName} created successfully!");

                return RedirectToAction(nameof(Index));
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
                ViewBag.Managers = await _employeeService.GetAllEmployeesAsync();
                return View(createDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee");
                return HandleException(ex, "Failed to create employee");
            }
        }

        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    return NotFound();
                }

                var updateDto = new UpdateEmployeeDto
                {
                    Id = employee.Id,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    MiddleName = employee.MiddleName,
                    Email = employee.Email,
                    Phone = employee.Phone,
                    Mobile = employee.Mobile,
                    Address = employee.Address,
                    City = employee.City,
                    State = employee.State,
                    Country = employee.Country,
                    //PostalCode = employee.PostalCode,
                    JobTitle = employee.JobTitle,
                    JobGrade = employee.JobGrade,
                    Salary = employee.Salary,
                    DepartmentId = employee.DepartmentId,
                    ManagerId = employee.ManagerId,
                    Status = employee.Status,
                   // BankName = employee.BankName,
                   // BankAccount = employee.BankAccount,
                   // BankBranch = employee.BankBranch
                };

                ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
                ViewBag.Managers = await _employeeService.GetAllEmployeesAsync();

                return View(updateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit form for ID: {Id}", id);
                return HandleException(ex, "Failed to load edit form");
            }
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateEmployeeDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest();
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
                    ViewBag.Managers = await _employeeService.GetAllEmployeesAsync();
                    return View(updateDto);
                }

                var employee = await _employeeService.UpdateEmployeeAsync(updateDto);
                AddSuccessMessage($"Employee {employee.FullName} updated successfully!");

                return RedirectToAction(nameof(Index));
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
                ViewBag.Managers = await _employeeService.GetAllEmployeesAsync();
                return View(updateDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee ID: {Id}", id);
                return HandleException(ex, "Failed to update employee");
            }
        }

        // POST: Employee/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _employeeService.DeleteEmployeeAsync(id);
                AddSuccessMessage("Employee deleted successfully!");

                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee ID: {Id}", id);
                return HandleException(ex, "Failed to delete employee");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckEmail(string email, int? id)
        {
            try
            {
                var isUnique = await _employeeService.IsEmailUniqueAsync(email, id);
                return Json(isUnique);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email uniqueness");
                return Json(false);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Export(EmployeeSearchDto searchDto)
        {
            try
            {
                var fileData = await _employeeService.ExportEmployeesToExcelAsync(searchDto);
                return File(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Employees_{DateTime.Now:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting employees");
                return HandleException(ex, "Failed to export employees");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    AddErrorMessage("Please select a file to import");
                    return RedirectToAction(nameof(Index));
                }

                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var count = await _employeeService.ImportEmployeesFromExcelAsync(memoryStream.ToArray());

                AddSuccessMessage($"{count} employees imported successfully!");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing employees");
                return HandleException(ex, "Failed to import employees");
            }
        }
    }
}