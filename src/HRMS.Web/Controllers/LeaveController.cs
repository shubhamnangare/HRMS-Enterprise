using HRMS.Core.DTOs.Leave;
using HRMS.Core.Enums;
using HRMS.Services.Employees;
using HRMS.Services.Leave;
using HRMS.Services.Leave.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Web.Controllers
{
    [Authorize]
    public class LeaveController : BaseController
    {
        private readonly ILeaveService _leaveService;
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<LeaveController> _logger;

        public LeaveController(
            ILeaveService leaveService,
            IEmployeeService employeeService,
            ILogger<LeaveController> logger)
        {
            _leaveService = leaveService;
            _employeeService = employeeService;
            _logger = logger;
        }

        // GET: Leave
        public async Task<IActionResult> Index(LeaveStatus? status = null)
        {
            try
            {
                var isAdmin = User.IsInRole("Admin") || User.IsInRole("HR");
                IEnumerable<LeaveRequestDto> leaves;

                if (isAdmin)
                {
                    // Admin sees all leaves
                    leaves = await _leaveService.GetAllLeaveRequestsAsync();

                    if (status.HasValue)
                    {
                        leaves = leaves.Where(l => l.StatusEnum == status.Value);
                    }
                }
                else
                {
                    // Regular employee sees only their leaves
                    var employee = await _employeeService.GetCurrentEmployeeAsync(User);
                    leaves = await _leaveService.GetLeaveRequestsByEmployeeAsync(employee.Id);
                }

                ViewBag.Statuses = Enum.GetValues<LeaveStatus>();
                ViewBag.SelectedStatus = status;
                return View(leaves);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading leaves");
                return HandleException(ex, "Failed to load leaves");
            }
        }

        // GET: Leave/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var employee = await _employeeService.GetCurrentEmployeeAsync(User);
                var balance = await _leaveService.GetLeaveBalanceAsync(employee.Id, DateTime.Now.Year);
                ViewBag.LeaveBalance = balance;
                ViewBag.LeaveTypes = Enum.GetValues<LeaveType>();
                return View(new CreateLeaveRequestDto { EmployeeId = employee.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading leave creation form");
                return HandleException(ex, "Failed to load leave creation form");
            }
        }

        // POST: Leave/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateLeaveRequestDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var employee = await _employeeService.GetCurrentEmployeeAsync(User);
                    ViewBag.LeaveBalance = await _leaveService.GetLeaveBalanceAsync(employee.Id, DateTime.Now.Year);
                    ViewBag.LeaveTypes = Enum.GetValues<LeaveType>();
                    return View(createDto);
                }

                var leave = await _leaveService.CreateLeaveRequestAsync(createDto);
                AddSuccessMessage($"Leave request submitted successfully! ({leave.LeaveType} from {leave.StartDate:dd/MM/yyyy} to {leave.EndDate:dd/MM/yyyy})");
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                var employee = await _employeeService.GetCurrentEmployeeAsync(User);
                ViewBag.LeaveBalance = await _leaveService.GetLeaveBalanceAsync(employee.Id, DateTime.Now.Year);
                ViewBag.LeaveTypes = Enum.GetValues<LeaveType>();
                return View(createDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating leave request");
                return HandleException(ex, "Failed to create leave request");
            }
        }

        // GET: Leave/Balance
        public async Task<IActionResult> Balance()
        {
            try
            {
                var employee = await _employeeService.GetCurrentEmployeeAsync(User);
                var balance = await _leaveService.GetLeaveBalanceAsync(employee.Id, DateTime.Now.Year);
                return View(balance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading leave balance");
                return HandleException(ex, "Failed to load leave balance");
            }
        }

        // GET: Leave/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var leave = await _leaveService.GetLeaveRequestByIdAsync(id);
                if (leave == null)
                {
                    return NotFound();
                }

                // Check authorization
                var isAdmin = User.IsInRole("Admin") || User.IsInRole("HR");
                var employee = await _employeeService.GetCurrentEmployeeAsync(User);

                if (!isAdmin && leave.EmployeeId != employee.Id)
                {
                    return Forbid();
                }

                return View(leave);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading leave details for ID: {Id}", id);
                return HandleException(ex, "Failed to load leave details");
            }
        }

        // POST: Leave/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var result = await _leaveService.CancelLeaveRequestAsync(id);
                if (result)
                {
                    AddSuccessMessage("Leave request cancelled successfully!");
                }
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                AddErrorMessage(ex.Message);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling leave request {Id}", id);
                return HandleException(ex, "Failed to cancel leave request");
            }
        }

        // GET: Leave/Approve/5 (Admin/HR only)
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var leave = await _leaveService.GetLeaveRequestByIdAsync(id);
                if (leave == null)
                {
                    AddWarningMessage("Leave request not found");
                    return RedirectToAction(nameof(Index));
                }

                if (leave.StatusEnum != LeaveStatus.Pending)
                {
                    AddWarningMessage($"Leave request is already {leave.Status}");
                    return RedirectToAction(nameof(Index));
                }

                return View(leave);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading approve form for ID: {Id}", id);
                return HandleException(ex, "Failed to load approve form");
            }
        }

        // POST: Leave/Approve/5 (Admin/HR only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Approve(int id, string? remarks)
        {
            try
            {
                var approver = await _employeeService.GetCurrentEmployeeAsync(User);
                var leave = await _leaveService.ApproveLeaveRequestAsync(id, approver.Id, remarks);
                AddSuccessMessage($"Leave request for {leave.EmployeeName} approved!");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving leave request {Id}", id);
                return HandleException(ex, "Failed to approve leave request");
            }
        }

        // GET: Leave/Reject/5 (Admin/HR only)
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                var leave = await _leaveService.GetLeaveRequestByIdAsync(id);
                if (leave == null)
                {
                    AddWarningMessage("Leave request not found");
                    return RedirectToAction(nameof(Index));
                }

                if (leave.StatusEnum != LeaveStatus.Pending)
                {
                    AddWarningMessage($"Leave request is already {leave.Status}");
                    return RedirectToAction(nameof(Index));
                }

                return View(leave);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading reject form for ID: {Id}", id);
                return HandleException(ex, "Failed to load reject form");
            }
        }

        // POST: Leave/Reject/5 (Admin/HR only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Reject(int id, string remarks)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(remarks))
                {
                    ModelState.AddModelError("", "Rejection reason is required");
                    var leaveView = await _leaveService.GetLeaveRequestByIdAsync(id);
                    return View(leaveView);
                }

                var approver = await _employeeService.GetCurrentEmployeeAsync(User);
                var leave = await _leaveService.RejectLeaveRequestAsync(id, approver.Id, remarks);
                AddSuccessMessage($"Leave request for {leave.EmployeeName} rejected!");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting leave request {Id}", id);
                return HandleException(ex, "Failed to reject leave request");
            }
        }
    }
}