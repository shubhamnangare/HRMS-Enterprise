using AutoMapper;
using ClosedXML.Excel;
using FluentValidation;
using HRMS.Core.Entities;
using HRMS.Core.Enums;
using HRMS.Core.Interfaces.Repositories;
using HRMS.Services.Employees.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace HRMS.Services.Employees
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeService> _logger;
        private readonly IMemoryCache _cache;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IValidator<CreateEmployeeDto> _createValidator;
        private readonly IValidator<UpdateEmployeeDto> _updateValidator;

        public EmployeeService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<EmployeeService> logger,
            IMemoryCache cache,
            UserManager<IdentityUser> userManager,
            IValidator<CreateEmployeeDto> createValidator,
            IValidator<UpdateEmployeeDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
            _userManager = userManager;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<Employee> GetCurrentEmployeeAsync(ClaimsPrincipal user)
        {
            var identityUser = await _userManager.GetUserAsync(user);
            if (identityUser == null)
                throw new UnauthorizedAccessException("User not found");

            var employee = await _unitOfWork.Employees.GetByEmailAsync(identityUser.Email);
            if (employee == null)
                throw new InvalidOperationException($"No employee record found for user {identityUser.Email}");

            return employee;
        }

        public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
        {
            var cacheKey = $"employee_{id}";
            if (_cache.TryGetValue(cacheKey, out EmployeeDto? cachedEmployee))
                return cachedEmployee;

            var employee = await _unitOfWork.Employees.GetEmployeeWithDetailsAsync(id);
            if (employee == null)
                return null;

            var employeeDto = _mapper.Map<EmployeeDto>(employee);
            _cache.Set(cacheKey, employeeDto, TimeSpan.FromMinutes(5));

            return employeeDto;
        }

        public async Task<IEnumerable<EmployeeListDto>> GetAllEmployeesAsync()
        {
            var employees = await _unitOfWork.Employees.GetAllAsync();
            return _mapper.Map<IEnumerable<EmployeeListDto>>(employees);
        }

        public async Task<IEnumerable<EmployeeListDto>> SearchEmployeesAsync(EmployeeSearchDto searchDto)
        {
            var employees = await _unitOfWork.Employees.SearchEmployeesAsync(
                searchDto.SearchTerm,
                searchDto.DepartmentId,
                searchDto.Status);

            return _mapper.Map<IEnumerable<EmployeeListDto>>(employees);
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createDto)
        {
            var validationResult = await _createValidator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var isEmailUnique = await _unitOfWork.Employees.IsEmailUniqueAsync(createDto.Email);
            if (!isEmailUnique)
                throw new InvalidOperationException($"Email {createDto.Email} is already in use");

            var employee = _mapper.Map<Employee>(createDto);
            employee.EmployeeCode = await GenerateEmployeeCodeAsync();
            employee.Status = EmployeeStatus.Active;
            employee.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.CompleteAsync();

            _cache.Remove("employees_list");

            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<EmployeeDto> UpdateEmployeeAsync(UpdateEmployeeDto updateDto)
        {
            var validationResult = await _updateValidator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var employee = await _unitOfWork.Employees.GetByIdAsync(updateDto.Id);
            if (employee == null)
                throw new KeyNotFoundException($"Employee with ID {updateDto.Id} not found");

            if (employee.Email != updateDto.Email)
            {
                var isEmailUnique = await _unitOfWork.Employees.IsEmailUniqueAsync(updateDto.Email, updateDto.Id);
                if (!isEmailUnique)
                    throw new InvalidOperationException($"Email {updateDto.Email} is already in use");
            }

            _mapper.Map(updateDto, employee);
            employee.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Employees.Update(employee);
            await _unitOfWork.CompleteAsync();

            _cache.Remove($"employee_{employee.Id}");
            _cache.Remove("employees_list");

            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
                throw new KeyNotFoundException($"Employee with ID {id} not found");

            employee.IsDeleted = true;
            employee.Status = EmployeeStatus.Terminated;
            employee.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Employees.Update(employee);
            await _unitOfWork.CompleteAsync();

            _cache.Remove($"employee_{id}");
            _cache.Remove("employees_list");

            return true;
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
        {
            return await _unitOfWork.Employees.IsEmailUniqueAsync(email, excludeId);
        }

        public async Task<bool> IsEmployeeCodeUniqueAsync(string code, int? excludeId = null)
        {
            return await _unitOfWork.Employees.IsEmployeeCodeUniqueAsync(code, excludeId);
        }

        public async Task<int> GetEmployeeCountAsync()
        {
            return await _unitOfWork.Employees.CountAsync(e => !e.IsDeleted);
        }

        public async Task<IEnumerable<EmployeeListDto>> GetEmployeesByDepartmentAsync(int departmentId)
        {
            var employees = await _unitOfWork.Employees.GetEmployeesByDepartmentAsync(departmentId);
            return _mapper.Map<IEnumerable<EmployeeListDto>>(employees);
        }

        public async Task<IEnumerable<EmployeeListDto>> GetEmployeesByManagerAsync(int managerId)
        {
            var employees = await _unitOfWork.Employees.GetEmployeesByManagerAsync(managerId);
            return _mapper.Map<IEnumerable<EmployeeListDto>>(employees);
        }

        // ==================== EXPORT METHOD ====================

        public async Task<byte[]> ExportEmployeesToExcelAsync(EmployeeSearchDto searchDto)
        {
            var employees = await _unitOfWork.Employees.SearchEmployeesAsync(
                searchDto.SearchTerm,
                searchDto.DepartmentId,
                searchDto.Status);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Employees");

            // Headers
            var headers = new[] { "Employee Code", "First Name", "Last Name", "Email", "Department",
                "Job Title", "Status", "Hire Date", "Salary", "Phone" };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            }

            // Data rows
            int row = 2;
            foreach (var emp in employees)
            {
                worksheet.Cell(row, 1).Value = emp.EmployeeCode;
                worksheet.Cell(row, 2).Value = emp.FirstName;
                worksheet.Cell(row, 3).Value = emp.LastName;
                worksheet.Cell(row, 4).Value = emp.Email;
                worksheet.Cell(row, 5).Value = emp.Department?.Name ?? "N/A";
                worksheet.Cell(row, 6).Value = emp.JobTitle;
                worksheet.Cell(row, 7).Value = emp.Status.ToString();
                worksheet.Cell(row, 8).Value = emp.HireDate.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 9).Value = emp.Salary;
                worksheet.Cell(row, 10).Value = emp.Phone;
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        // ==================== IMPORT METHOD ====================

        public async Task<int> ImportEmployeesFromExcelAsync(byte[] fileData)
        {
            var importedCount = 0;
            var errors = new List<string>();

            using var stream = new MemoryStream(fileData);
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RowsUsed().Skip(1);

            // Get departments for lookup
            var departments = await _unitOfWork.Departments.GetAllAsync();
            var departmentLookup = departments.ToDictionary(d => d.Name.ToLower(), d => d.Id);
            var departmentCodeLookup = departments.ToDictionary(d => d.Code.ToLower(), d => d.Id);

            foreach (var row in rows)
            {
                try
                {
                    var employeeCode = row.Cell(1).GetString()?.Trim();
                    var firstName = row.Cell(2).GetString()?.Trim();
                    var lastName = row.Cell(3).GetString()?.Trim();
                    var email = row.Cell(4).GetString()?.Trim();
                    var departmentName = row.Cell(5).GetString()?.Trim();
                    var jobTitle = row.Cell(6).GetString()?.Trim();
                    var statusText = row.Cell(7).GetString()?.Trim();
                    var hireDateText = row.Cell(8).GetString()?.Trim();
                    var salaryText = row.Cell(9).GetString()?.Trim();
                    var phone = row.Cell(10).GetString()?.Trim();

                    // Validate required fields
                    if (string.IsNullOrEmpty(employeeCode))
                    {
                        errors.Add($"Row {row.RowNumber()}: Employee Code is required");
                        continue;
                    }
                    if (string.IsNullOrEmpty(firstName))
                    {
                        errors.Add($"Row {row.RowNumber()}: First Name is required");
                        continue;
                    }
                    if (string.IsNullOrEmpty(lastName))
                    {
                        errors.Add($"Row {row.RowNumber()}: Last Name is required");
                        continue;
                    }
                    if (string.IsNullOrEmpty(email))
                    {
                        errors.Add($"Row {row.RowNumber()}: Email is required");
                        continue;
                    }

                    // Check duplicates
                    if (await _unitOfWork.Employees.GetByEmployeeCodeAsync(employeeCode) != null)
                    {
                        errors.Add($"Row {row.RowNumber()}: Employee code '{employeeCode}' already exists");
                        continue;
                    }
                    if (await _unitOfWork.Employees.GetByEmailAsync(email) != null)
                    {
                        errors.Add($"Row {row.RowNumber()}: Email '{email}' already exists");
                        continue;
                    }

                    // Get department ID
                    int? departmentId = null;
                    if (!string.IsNullOrEmpty(departmentName))
                    {
                        var deptKey = departmentName.ToLower();
                        if (departmentLookup.TryGetValue(deptKey, out int deptId))
                            departmentId = deptId;
                        else if (departmentCodeLookup.TryGetValue(deptKey, out int deptCodeId))
                            departmentId = deptCodeId;
                        else
                            errors.Add($"Row {row.RowNumber()}: Department '{departmentName}' not found");
                    }

                    // Parse hire date
                    DateTime hireDate = DateTime.TryParse(hireDateText, out var parsedHireDate) ? parsedHireDate : DateTime.Today;

                    // Parse salary
                    decimal salary = decimal.TryParse(salaryText, out var parsedSalary) ? parsedSalary : 0;

                    // Parse status
                    var status = EmployeeStatus.Active;
                    if (!string.IsNullOrEmpty(statusText))
                        Enum.TryParse<EmployeeStatus>(statusText, true, out status);

                    // Create employee
                    var employee = new Employee
                    {
                        EmployeeCode = employeeCode,
                        FirstName = firstName,
                        LastName = lastName,
                        Email = email,
                        DepartmentId = departmentId,
                        JobTitle = string.IsNullOrEmpty(jobTitle) ? "Employee" : jobTitle,
                        HireDate = hireDate,
                        Salary = salary,
                        Phone = phone,
                        Status = status,
                        EmploymentType = EmploymentType.Permanent,
                        Gender = Gender.Male,
                        MaritalStatus = MaritalStatus.Single,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.Employees.AddAsync(employee);
                    importedCount++;

                    if (importedCount % 50 == 0)
                        await _unitOfWork.CompleteAsync();
                }
                catch (Exception ex)
                {
                    errors.Add($"Row {row.RowNumber()}: {ex.Message}");
                }
            }

            if (importedCount > 0)
                await _unitOfWork.CompleteAsync();

            if (errors.Any())
                _logger.LogWarning("Import completed with {ErrorCount} errors", errors.Count);

            return importedCount;
        }

        // ==================== TEMPLATE METHOD ====================

        public async Task<byte[]> GetImportTemplateAsync()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("EmployeeTemplate");

            // Headers
            var headers = new[] { "Employee Code*", "First Name*", "Last Name*", "Email*", "Department Name",
                "Job Title", "Status", "Hire Date", "Salary", "Phone" };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            }

            // Sample data
            worksheet.Cell(2, 1).Value = "EMP001";
            worksheet.Cell(2, 2).Value = "John";
            worksheet.Cell(2, 3).Value = "Doe";
            worksheet.Cell(2, 4).Value = "john.doe@example.com";
            worksheet.Cell(2, 5).Value = "Information Technology";
            worksheet.Cell(2, 6).Value = "Software Engineer";
            worksheet.Cell(2, 7).Value = "Active";
            worksheet.Cell(2, 8).Value = "2024-01-15";
            worksheet.Cell(2, 9).Value = "60000";
            worksheet.Cell(2, 10).Value = "+1-555-0101";

            worksheet.Cell(3, 1).Value = "EMP002";
            worksheet.Cell(3, 2).Value = "Jane";
            worksheet.Cell(3, 3).Value = "Smith";
            worksheet.Cell(3, 4).Value = "jane.smith@example.com";
            worksheet.Cell(3, 5).Value = "Human Resources";
            worksheet.Cell(3, 6).Value = "HR Specialist";
            worksheet.Cell(3, 7).Value = "Active";
            worksheet.Cell(3, 8).Value = "2024-02-01";
            worksheet.Cell(3, 9).Value = "55000";
            worksheet.Cell(3, 10).Value = "+1-555-0102";

            // Notes column
            worksheet.Cell(1, 11).Value = "Notes";
            worksheet.Cell(1, 11).Style.Font.Bold = true;
            worksheet.Cell(2, 11).Value = "* Required fields";
            worksheet.Cell(3, 11).Value = "Department Name must match existing department names";
            worksheet.Cell(4, 11).Value = "Status: Active, Inactive, OnLeave, Terminated, Resigned";
            worksheet.Cell(5, 11).Value = "Date format: YYYY-MM-DD";

            // Get departments for reference
            var departments = await _unitOfWork.Departments.GetAllAsync();
            var departmentList = string.Join(", ", departments.Select(d => d.Name));
            worksheet.Cell(6, 11).Value = $"Available Departments: {departmentList}";

            worksheet.Columns().AdjustToContents();
            worksheet.SheetView.FreezeRows(1);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        // ==================== HELPER METHODS ====================

        private async Task<string> GenerateEmployeeCodeAsync()
        {
            var year = DateTime.Now.Year.ToString().Substring(2);
            var lastEmployee = await _unitOfWork.Employees
                .FindAsync(e => e.EmployeeCode.StartsWith($"EMP{year}"));

            if (!lastEmployee.Any())
                return $"EMP{year}0001";

            var lastCode = lastEmployee.Select(e => e.EmployeeCode).OrderByDescending(c => c).First();
            var lastNumber = int.Parse(lastCode.Substring(5));
            return $"EMP{year}{(lastNumber + 1):D4}";
        }
    }
}