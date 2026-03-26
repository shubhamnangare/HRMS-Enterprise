using AutoMapper;
using HRMS.Core.DTOs.Leave;
using HRMS.Core.Entities;
using HRMS.Core.Enums;
using HRMS.Core.Interfaces.Repositories;
using HRMS.Services.Leave.Dtos;
using Microsoft.Extensions.Logging;

namespace HRMS.Services.Leave
{
    public class LeaveService : ILeaveService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<LeaveService> _logger;

        public LeaveService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<LeaveService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        // Get leave request by ID
        public async Task<LeaveRequestDto?> GetLeaveRequestByIdAsync(int id)
        {
            try
            {
                var leave = await _unitOfWork.Leaves.GetByIdAsync(id);
                if (leave == null || leave.IsDeleted)
                    return null;

                return _mapper.Map<LeaveRequestDto>(leave);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting leave request with ID {LeaveId}", id);
                throw;
            }
        }

        // Get leaves by employee
        public async Task<IEnumerable<LeaveRequestDto>> GetLeaveRequestsByEmployeeAsync(int employeeId)
        {
            try
            {
                var leaves = await _unitOfWork.Leaves.GetByEmployeeAsync(employeeId);
                return _mapper.Map<IEnumerable<LeaveRequestDto>>(leaves);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting leave requests for employee {EmployeeId}", employeeId);
                throw;
            }
        }

        public async Task<IEnumerable<LeaveRequestDto>> GetAllLeaveRequestsAsync()
        {
            try
            {
                var leaves = await _unitOfWork.Leaves.GetAllAsync();
                return _mapper.Map<IEnumerable<LeaveRequestDto>>(leaves);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all leave requests");
                throw;
            }
        }

        // Create leave request
        public async Task<LeaveRequestDto> CreateLeaveRequestAsync(CreateLeaveRequestDto createDto)
        {
            try
            {
                // Validate leave request
                var canApply = await CanApplyLeaveAsync(
                    createDto.EmployeeId,
                    createDto.LeaveType,
                    createDto.StartDate,
                    createDto.EndDate);

                if (!canApply)
                {
                    throw new InvalidOperationException("Cannot apply for leave. Check balance or overlapping dates.");
                }

                // Calculate total days
                var totalDays = CalculateTotalDays(createDto.StartDate, createDto.EndDate);

                var leave = new LeaveRequest
                {
                    EmployeeId = createDto.EmployeeId,
                    LeaveType = createDto.LeaveType,
                    StartDate = createDto.StartDate,
                    EndDate = createDto.EndDate,
                    TotalDays = totalDays,
                    Reason = createDto.Reason,
                    IsPaid = createDto.IsPaid,
                    Status = LeaveStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Leaves.AddAsync(leave);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Leave request created for employee {EmployeeId}", createDto.EmployeeId);

                return _mapper.Map<LeaveRequestDto>(leave);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating leave request for employee {EmployeeId}", createDto.EmployeeId);
                throw;
            }
        }

        // Update leave request
        public async Task<LeaveRequestDto> UpdateLeaveRequestAsync(UpdateLeaveRequestDto updateDto)
        {
            try
            {
                var leave = await _unitOfWork.Leaves.GetByIdAsync(updateDto.Id);
                if (leave == null || leave.IsDeleted)
                    throw new KeyNotFoundException($"Leave request with ID {updateDto.Id} not found");

                if (leave.Status != LeaveStatus.Pending)
                    throw new InvalidOperationException($"Cannot update leave that is already {leave.Status}");

                if (updateDto.StartDate.HasValue && updateDto.EndDate.HasValue)
                {
                    // Check overlap
                    var hasOverlap = await _unitOfWork.Leaves.HasOverlapAsync(
                        leave.EmployeeId,
                        updateDto.StartDate.Value,
                        updateDto.EndDate.Value,
                        updateDto.Id);

                    if (hasOverlap)
                        throw new InvalidOperationException("Leave dates overlap with existing request");

                    leave.StartDate = updateDto.StartDate.Value;
                    leave.EndDate = updateDto.EndDate.Value;
                    leave.TotalDays = CalculateTotalDays(updateDto.StartDate.Value, updateDto.EndDate.Value);
                }

                if (updateDto.LeaveType.HasValue)
                    leave.LeaveType = updateDto.LeaveType.Value;

                if (!string.IsNullOrEmpty(updateDto.Reason))
                    leave.Reason = updateDto.Reason;

                if (updateDto.IsPaid.HasValue)
                    leave.IsPaid = updateDto.IsPaid.Value;

                leave.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Leaves.Update(leave);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Leave request {LeaveId} updated", leave.Id);

                return _mapper.Map<LeaveRequestDto>(leave);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating leave request {LeaveId}", updateDto.Id);
                throw;
            }
        }

        // Cancel leave request
        public async Task<bool> CancelLeaveRequestAsync(int id)
        {
            try
            {
                var leave = await _unitOfWork.Leaves.GetByIdAsync(id);
                if (leave == null || leave.IsDeleted)
                    throw new KeyNotFoundException($"Leave request with ID {id} not found");

                if (leave.Status != LeaveStatus.Pending && leave.Status != LeaveStatus.Approved)
                    throw new InvalidOperationException($"Cannot cancel leave with status {leave.Status}");

                if (leave.Status == LeaveStatus.Approved && leave.StartDate <= DateTime.Today)
                    throw new InvalidOperationException("Cannot cancel approved leave that has already started");

                leave.Status = LeaveStatus.Cancelled;
                leave.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Leaves.Update(leave);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Leave request {LeaveId} cancelled", id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling leave request {LeaveId}", id);
                throw;
            }
        }

        // Approve leave request
        public async Task<LeaveRequestDto> ApproveLeaveRequestAsync(int id, int approverId, string? remarks = null)
        {
            try
            {
                var leave = await _unitOfWork.Leaves.GetByIdAsync(id);
                if (leave == null || leave.IsDeleted)
                    throw new KeyNotFoundException($"Leave request with ID {id} not found");

                if (leave.Status != LeaveStatus.Pending)
                    throw new InvalidOperationException($"Cannot approve leave that is already {leave.Status}");

                leave.Status = LeaveStatus.Approved;
                leave.ApprovedById = approverId;
                leave.ApprovedDate = DateTime.UtcNow;
                leave.Remarks = remarks;
                leave.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Leaves.Update(leave);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Leave request {LeaveId} approved by {ApproverId}", id, approverId);

                return _mapper.Map<LeaveRequestDto>(leave);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving leave request {LeaveId}", id);
                throw;
            }
        }

        // Reject leave request
        public async Task<LeaveRequestDto> RejectLeaveRequestAsync(int id, int approverId, string remarks)
        {
            try
            {
                var leave = await _unitOfWork.Leaves.GetByIdAsync(id);
                if (leave == null || leave.IsDeleted)
                    throw new KeyNotFoundException($"Leave request with ID {id} not found");

                if (leave.Status != LeaveStatus.Pending)
                    throw new InvalidOperationException($"Cannot reject leave that is already {leave.Status}");

                leave.Status = LeaveStatus.Rejected;
                leave.ApprovedById = approverId;
                leave.ApprovedDate = DateTime.UtcNow;
                leave.Remarks = remarks;
                leave.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Leaves.Update(leave);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Leave request {LeaveId} rejected by {ApproverId}", id, approverId);

                return _mapper.Map<LeaveRequestDto>(leave);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting leave request {LeaveId}", id);
                throw;
            }
        }

        // Get leave balance
        public async Task<LeaveBalanceDto> GetLeaveBalanceAsync(int employeeId, int year)
        {
            try
            {
                var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
                if (employee == null)
                    throw new KeyNotFoundException($"Employee with ID {employeeId} not found");

                var balance = new LeaveBalanceDto
                {
                    EmployeeId = employeeId,
                    EmployeeName = $"{employee.FirstName} {employee.LastName}",
                    Balances = new Dictionary<LeaveType, LeaveTypeBalance>()
                };

                foreach (LeaveType leaveType in Enum.GetValues(typeof(LeaveType)))
                {
                    var entitlement = GetLeaveEntitlement(leaveType, employee.HireDate, year);
                    var used = await _unitOfWork.Leaves.GetUsedLeaveDaysAsync(employeeId, leaveType, year);

                    balance.Balances[leaveType] = new LeaveTypeBalance
                    {
                        LeaveType = leaveType,
                        LeaveTypeName = leaveType.ToString(),
                        TotalEntitled = entitlement,
                        Used = used,
                        Remaining = entitlement - used,
                        IsPaid = IsLeavePaid(leaveType)
                    };
                }

                return balance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting leave balance for employee {EmployeeId}", employeeId);
                throw;
            }
        }

        // Check if employee can apply for leave
        public async Task<bool> CanApplyLeaveAsync(int employeeId, LeaveType leaveType, DateTime startDate, DateTime endDate)
        {
            try
            {
                // Check past dates
                if (startDate < DateTime.Today)
                    return false;

                // Check start date before end date
                if (startDate > endDate)
                    return false;

                // Check overlapping leaves
                var hasOverlap = await _unitOfWork.Leaves.HasOverlapAsync(employeeId, startDate, endDate);
                if (hasOverlap)
                    return false;

                // Check leave balance
                var balance = await GetLeaveBalanceAsync(employeeId, DateTime.Now.Year);
                var totalDays = CalculateTotalDays(startDate, endDate);

                if (balance.Balances.TryGetValue(leaveType, out var leaveBalance))
                {
                    return totalDays <= leaveBalance.Remaining;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if employee {EmployeeId} can apply for leave", employeeId);
                throw;
            }
        }

        // Get pending leaves (for HR/Admin)
        public async Task<IEnumerable<LeaveRequestDto>> GetPendingLeavesAsync()
        {
            try
            {
                var leaves = await _unitOfWork.Leaves.GetPendingLeavesAsync();
                return _mapper.Map<IEnumerable<LeaveRequestDto>>(leaves);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending leaves");
                throw;
            }
        }

        // Get employees on leave on specific date
        public async Task<IEnumerable<Employee>> GetEmployeesOnLeaveAsync(DateTime date)
        {
            try
            {
                return await _unitOfWork.Leaves.GetEmployeesEntitiesOnLeaveAsync(date);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employees on leave for date {Date}", date);
                throw;
            }
        }

        // Helper methods
        private decimal CalculateTotalDays(DateTime startDate, DateTime endDate)
        {
            decimal days = 0;
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    days++;
                }
            }
            return days;
        }

        private decimal GetLeaveEntitlement(LeaveType leaveType, DateTime hireDate, int year)
        {
            var yearsOfService = year - hireDate.Year;

            return leaveType switch
            {
                LeaveType.Annual => yearsOfService < 1 ? 12 :
                                    yearsOfService < 3 ? 15 :
                                    yearsOfService < 5 ? 18 : 21,
                LeaveType.Sick => 12,
                LeaveType.Maternity => 90,
                LeaveType.Paternity => 10,
                LeaveType.Unpaid => 30,
                LeaveType.Compensatory => 30,
                LeaveType.Bereavement => 5,
                LeaveType.Marriage => 5,
                _ => 0
            };
        }

        private bool IsLeavePaid(LeaveType leaveType)
        {
            return leaveType switch
            {
                LeaveType.Unpaid => false,
                _ => true
            };
        }
    }
}