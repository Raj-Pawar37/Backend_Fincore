using AutoMapper;
using Backend_Fincore.Application.DTOs.PurchaseRequisition;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Backend_Fincore.Response;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Backend_Fincore.Infrastucture.Service
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
            if (userId <= 0)
            {
                return new ApiResponse<List<PurchaseRequisitionResponseDto>> { Success = false, Message = "User ID is missing or invalid" };
            }

            var user = await _context.User
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return new ApiResponse<List<PurchaseRequisitionResponseDto>> { Success = false, Message = "User ID not found" };
            }

            string roleName = user.Role.RoleName;

            if (roleName == "User")
            {
                return new ApiResponse<List<PurchaseRequisitionResponseDto>> { Success = false, Message = "You do not have permission to view PRs" };
            }

            IQueryable<PurchaseRequisition> query = _context.PurchaseRequisition.AsQueryable();

            if (roleName == "Manager")
            {
                var employee = await _context.Employee.FirstOrDefaultAsync(e => e.EmployeeId == user.MasterId);

                if (employee == null)
                {
                    return new ApiResponse<List<PurchaseRequisitionResponseDto>> { Success = false, Message = "Manager employee record not found" };
                }

                int managerDeptId = employee.DepartmentId;

                query = query.Where(pr => _context.CapexRequest.Any(cr =>
                                          cr.CapexRequestId == pr.CapexRequestId &&
                                          _context.User.Any(u =>
                                              u.UserId == cr.RequestedBy &&
                                              u.MasterType == "Employee" &&
                                              _context.Employee.Any(emp =>
                                                  emp.EmployeeId == u.MasterId &&
                                                  emp.DepartmentId == managerDeptId))));
            }
            
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

        public async Task<ApiResponse<PurchaseRequisitionResponseDto>> UpdateAsync(int id, PurchaseRequisitionUpdateDto dto, int userId)
        {
            var pr = await _context.PurchaseRequisition.FindAsync(id);

            if (pr == null)
            {
                return new ApiResponse<PurchaseRequisitionResponseDto> { Success = false, Message = "PRId not found" };
            }

            if (!string.IsNullOrEmpty(dto.Title)) pr.Title = dto.Title;
            if (!string.IsNullOrEmpty(dto.PRNumber)) pr.PRNumber = dto.PRNumber;
            if (!string.IsNullOrEmpty(dto.Description)) pr.Description = dto.Description;
            if (!string.IsNullOrEmpty(dto.Status)) pr.Status = dto.Status;

           
            pr.ModifiedBy = userId;
            pr.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        public async Task<ApiResponse<List<PRDropdownResponseDto>>> GetPRDropdownAsync(string? searchText, int? departmentId)
        {
            IQueryable<PurchaseRequisition> query = _context.PurchaseRequisition.AsQueryable();

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

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(pr => pr.PRNumber.Contains(searchText) || pr.Title.Contains(searchText))
                             .Take(20);
            }

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