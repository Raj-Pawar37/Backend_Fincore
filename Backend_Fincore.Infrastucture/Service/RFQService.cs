using AutoMapper;
using Backend_Fincore.Application.DTOs.RFQ;
using Backend_Fincore.Application.Interfaces;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Backend_Fincore.Response;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Application.Services
{
    public class RFQService : IRFQService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public RFQService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ==========================================
        // 1. CREATE RFQ
        // ==========================================
        public async Task<ApiResponse<RFQResponseDto>> CreateAsync(RFQCreateDto dto)
        {
            // Rule 1: If RFQNumber already exists, throw error
            if (await _context.RFQ.AnyAsync(r => r.RFQNumber == dto.RFQNumber))
            {
                return new ApiResponse<RFQResponseDto> { Success = false, Message = "RFQ Number already exists." };
            }

            // Rule 2: If PRId doesn't exist, throw error
            if (!await _context.PurchaseRequisition.AnyAsync(pr => pr.PRId == dto.PRId))
            {
                return new ApiResponse<RFQResponseDto> { Success = false, Message = "Purchase Requisition ID not found." };
            }

            // Rule 3: If RFQ already exists with the same PRId, throw error
            if (await _context.RFQ.AnyAsync(r => r.PRId == dto.PRId))
            {
                return new ApiResponse<RFQResponseDto> { Success = false, Message = "An RFQ already exists for this Purchase Requisition." };
            }

            var rfq = new RFQ
            {
                RFQNumber = dto.RFQNumber,
                Title = dto.Title,
                Description = dto.Description,
                IssueDate = dto.IssueDate,
                ClosingDate = dto.ClosingDate,
                PRId = dto.PRId,
                Status = "Pending" // Rule 4: By default Status Pending
            };

            _context.RFQ.Add(rfq);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<RFQResponseDto>(rfq);
            return new ApiResponse<RFQResponseDto> { Success = true, Message = "RFQ created successfully", Data = responseDto, TotalNumberRecord = 1 };
        }

        // ==========================================
        // 2. READ ALL (ROLE BASED)
        // ==========================================
        public async Task<ApiResponse<List<RFQResponseDto>>> GetAllAsync(int userId)
        {
            if (userId <= 0)
                return new ApiResponse<List<RFQResponseDto>> { Success = false, Message = "User ID is missing or invalid." };

            // Fetch User and Role securely
            var user = await _context.User.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId);

            // Rule 1: If userID not found throw error
            if (user == null)
                return new ApiResponse<List<RFQResponseDto>> { Success = false, Message = "User ID not found." };

            string roleName = user.Role.RoleName;

            // Rule 2: User Role throws permission error
            if (roleName == "User")
                return new ApiResponse<List<RFQResponseDto>> { Success = false, Message = "You do not have permission to view RFQs." };

            IQueryable<RFQ> query = _context.RFQ.AsQueryable();

            // Rule 3: Manager filters RFQ by Department
            if (roleName == "Manager")
            {
                var employee = await _context.Employee.FirstOrDefaultAsync(e => e.EmployeeId == user.MasterId);
                if (employee == null)
                {
                    return new ApiResponse<List<RFQResponseDto>> { Success = false, Message = "Manager employee record not found." };
                }

                int managerDeptId = employee.DepartmentId;

                // Safely traverse RFQ -> PR -> Capex -> User -> Employee to check the department
                query = query.Where(rfq => _context.PurchaseRequisition.Any(pr =>
                                           pr.PRId == rfq.PRId &&
                                           _context.CapexRequest.Any(cr =>
                                               cr.CapexRequestId == pr.CapexRequestId &&
                                               _context.User.Any(u =>
                                                   u.UserId == cr.RequestedBy &&
                                                   u.MasterType == "Employee" &&
                                                   _context.Employee.Any(emp =>
                                                       emp.EmployeeId == u.MasterId &&
                                                       emp.DepartmentId == managerDeptId)))));
            }
            // Rule 4: CFO displays all RFQ
            else if (roleName != "CFO")
            {
                return new ApiResponse<List<RFQResponseDto>> { Success = false, Message = "Invalid Role." };
            }

            var rfqs = await query.ToListAsync();
            var rfqDtos = _mapper.Map<List<RFQResponseDto>>(rfqs).ToList();

            return new ApiResponse<List<RFQResponseDto>>
            {
                Success = true,
                Message = "RFQs fetched successfully",
                Data = rfqDtos,
                TotalNumberRecord = rfqDtos.Count
            };
        }

        // ==========================================
        // 3. READ BY RFQ ID
        // ==========================================
        public async Task<ApiResponse<RFQResponseDto>> GetByIdAsync(int id)
        {
            var rfq = await _context.RFQ.FindAsync(id);

            // Rule 1: If RFQId doesn't found then throw error
            if (rfq == null)
            {
                return new ApiResponse<RFQResponseDto> { Success = false, Message = "RFQ ID not found." };
            }

            var rfqDto = _mapper.Map<RFQResponseDto>(rfq);
            return new ApiResponse<RFQResponseDto> { Success = true, Data = rfqDto, TotalNumberRecord = 1 };
        }

        // ==========================================
        // 4. UPDATE RFQ
        // ==========================================
        public async Task<ApiResponse<RFQResponseDto>> UpdateAsync(int id, RFQUpdateDto dto)
        {
            var rfq = await _context.RFQ.FindAsync(id);

            // Rule 1: If RFQId doesn't found then throw error
            if (rfq == null)
            {
                return new ApiResponse<RFQResponseDto> { Success = false, Message = "RFQ ID not found." };
            }

            // Rule 3: Once status Open then you cant update the RFQ
            if (rfq.Status == "Open")
            {
                return new ApiResponse<RFQResponseDto> { Success = false, Message = "Cannot update RFQ once status is Open." };
            }

            // Rule 2: If Update RFQNumber already exist then throw error (exclude current RFQ)
            if (await _context.RFQ.AnyAsync(r => r.RFQNumber == dto.RFQNumber && r.RFQId != id))
            {
                return new ApiResponse<RFQResponseDto> { Success = false, Message = "RFQ Number already exists for another record." };
            }

            // Apply updates
            rfq.RFQNumber = dto.RFQNumber;
            rfq.Title = dto.Title;
            rfq.Description = dto.Description;
            rfq.IssueDate = dto.IssueDate;
            rfq.ClosingDate = dto.ClosingDate;

            if (!string.IsNullOrEmpty(dto.Status))
            {
                rfq.Status = dto.Status;
            }

            await _context.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        // ==========================================
        // 5. DELETE RFQ
        // ==========================================
        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var rfq = await _context.RFQ.FindAsync(id);

            // Rule: If RFQId doesnt found then throw error
            if (rfq == null)
            {
                return new ApiResponse<bool> { Success = false, Message = "RFQ ID not found.", Data = false };
            }

            // Rule: Once status Open then you cant Delete
            if (rfq.Status == "Open")
            {
                return new ApiResponse<bool> { Success = false, Message = "Cannot delete RFQ once status is Open.", Data = false };
            }

            // Rule: also delete RFQItems
            var rfqItems = await _context.RFQItem.Where(x => x.RFQId == id).ToListAsync();
            if (rfqItems.Any())
            {
                _context.RFQItem.RemoveRange(rfqItems);
            }

            // Rule: also delte RFQVendors
            var rfqVendors = await _context.RFQVendor.Where(x => x.RFQId == id).ToListAsync();
            if (rfqVendors.Any())
            {
                _context.RFQVendor.RemoveRange(rfqVendors);
            }

            // Rule: else delete the RFQ
            _context.RFQ.Remove(rfq);
            await _context.SaveChangesAsync();

            return new ApiResponse<bool> { Success = true, Message = "RFQ, Items, and Vendors deleted successfully.", Data = true };
        }


    }
}