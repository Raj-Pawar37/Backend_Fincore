using AutoMapper;
using Backend_Fincore.Application.DTOs.PurchaseRequisition;
using Backend_Fincore.Application.Interfaces;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Backend_Fincore.Response;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Backend_Fincore.Application.Services
{
    public class PurchaseRequisitionService : IPurchaseRequisitionService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PurchaseRequisitionService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<PurchaseRequisitionResponseDto>>> GetAllAsync(int userId)
        {
            // Validate input
            if (userId <= 0)
            {
                return new ApiResponse<List<PurchaseRequisitionResponseDto>> { Success = false, Message = "User ID is missing or invalid" };
            }

            //  Fetch the User from the database, and INCLUDE their Role so we know who they are
            var user = await _context.User
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return new ApiResponse<List<PurchaseRequisitionResponseDto>> { Success = false, Message = "User ID not found" };
            }

            // 3. Extract the Role Name safely from the database
            string roleName = user.Role.RoleName;

            if (roleName == "User")
            {
                return new ApiResponse<List<PurchaseRequisitionResponseDto>> { Success = false, Message = "You do not have permission to view PRs" };
            }

            IQueryable<PurchaseRequisition> query = _context.PurchaseRequisition.AsQueryable();

            // 4. Manager Logic (Securely fetch their department)
            if (roleName == "Manager")
            {
                // Find the Employee record linked to this User to get their Department
                var employee = await _context.Employee.FirstOrDefaultAsync(e => e.EmployeeId == user.MasterId);

                if (employee == null)
                {
                    return new ApiResponse<List<PurchaseRequisitionResponseDto>> { Success = false, Message = "Manager employee record not found" };
                }

                int managerDeptId = employee.DepartmentId;

                // Filter PRs where the requester is in the same department as the Manager
                query = query.Where(pr => _context.CapexRequest.Any(cr =>
                                          cr.CapexRequestId == pr.CapexRequestId &&
                                          _context.User.Any(u =>
                                              u.UserId == cr.RequestedBy &&
                                              u.MasterType == "Employee" &&
                                              _context.Employee.Any(emp =>
                                                  emp.EmployeeId == u.MasterId &&
                                                  emp.DepartmentId == managerDeptId))));
            }
            // 5. CFO Logic
            else if (roleName != "CFO")
            {
                return new ApiResponse<List<PurchaseRequisitionResponseDto>> { Success = false, Message = "Invalid Role" };
            }

            var prs = await query.ToListAsync();
            var prDtos = _mapper.Map<List<PurchaseRequisitionResponseDto>>(prs).ToList();

            return new ApiResponse<List<PurchaseRequisitionResponseDto>>
            {
                Success = true,
                Message = "Purchase Requisitions fetched successfully",
                Data = prDtos,
                TotalNumberRecord = prDtos.Count
            };
        }

        public async Task<ApiResponse<PurchaseRequisitionResponseDto>> GetByIdAsync(int id)
        {
            var pr = await _context.PurchaseRequisition.FindAsync(id);
            if (pr == null)
            {
                return new ApiResponse<PurchaseRequisitionResponseDto> { Success = false, Message = "Purchase Requisition not found" };
            }

            var prDto = _mapper.Map<PurchaseRequisitionResponseDto>(pr);
            return new ApiResponse<PurchaseRequisitionResponseDto> { Success = true, Data = prDto, TotalNumberRecord = 1 };
        }

        public async Task<ApiResponse<PurchaseRequisitionResponseDto>> UpdateAsync(int id, PurchaseRequisitionUpdateDto dto)
        {
            var pr = await _context.PurchaseRequisition.FindAsync(id);

            // Rule 2: If PRId not found throw error
            if (pr == null)
            {
                return new ApiResponse<PurchaseRequisitionResponseDto> { Success = false, Message = "PRId not found" };
            }

            // Rule 2: Only update Status, Name (Title), PRNumber, Description
            if (!string.IsNullOrEmpty(dto.Title)) pr.Title = dto.Title;
            if (!string.IsNullOrEmpty(dto.PRNumber)) pr.PRNumber = dto.PRNumber;
            if (!string.IsNullOrEmpty(dto.Description)) pr.Description = dto.Description;
            if (!string.IsNullOrEmpty(dto.Status)) pr.Status = dto.Status;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        public async Task<ApiResponse<List<PRDropdownResponseDto>>> GetPRDropdownAsync(string? searchText, int? departmentId)
        {
            IQueryable<PurchaseRequisition> query = _context.PurchaseRequisition.AsQueryable();

            // 1. Rule: If department is provided, filter by it. If null, skip this block (fetch all PRs).
            if (departmentId.HasValue && departmentId.Value > 0)
            {
                query = query.Where(pr => _context.CapexRequest.Any(cr =>
                                          cr.CapexRequestId == pr.CapexRequestId &&
                                          _context.User.Any(u =>
                                              u.UserId == cr.RequestedBy &&
                                              u.MasterType == "Employee" &&
                                              _context.Employee.Any(emp =>
                                                  emp.EmployeeId == u.MasterId &&
                                                  emp.DepartmentId == departmentId.Value))));
            }

            // 2. Rule: Apply search text and limit to Top 20
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(pr => pr.PRNumber.Contains(searchText) || pr.Title.Contains(searchText))
                             .Take(20);
            }
            // Optional safeguard: If you want to limit it to 20 even without search text to prevent huge payloads
            // else { query = query.Take(50); } 

            //  Project directly into the DTO (highly optimized SQL generation)
            var prs = await query.Select(pr => new PRDropdownResponseDto
            {
                PRId = pr.PRId,
                PRNumber = pr.PRNumber,
                Title = pr.Title
            }).ToListAsync();

            return new ApiResponse<List<PRDropdownResponseDto>>
            {
                Success = true,
                Message = "PR Dropdown fetched successfully",
                Data = prs,
                TotalNumberRecord = prs.Count
            };
        }
    }
}