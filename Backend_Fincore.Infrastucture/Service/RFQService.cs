using AutoMapper;
using Backend_Fincore.Application.DTOs.RFQ;
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
    public class RFQService : IRFQService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public RFQService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<RFQResponseDto>> CreateAsync(RFQCreateDto dto)
        {
            if (await _context.RFQ.AnyAsync(r => r.RFQNumber == dto.RFQNumber))
            {
                return new ApiResponse<RFQResponseDto> { Success = false, Message = "RFQ Number already exists." };
            }

            if (!await _context.PurchaseRequisition.AnyAsync(pr => pr.PRId == dto.PRId))
            {
                return new ApiResponse<RFQResponseDto> { Success = false, Message = "Purchase Requisition ID not found." };
            }

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
                Status = "Pending"
            };

            _context.RFQ.Add(rfq);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<RFQResponseDto>(rfq);
            return new ApiResponse<RFQResponseDto> { Success = true, Message = "RFQ created successfully", Data = responseDto, TotalNumberRecord = 1 };
        }

        public async Task<ApiResponse<List<RFQResponseDto>>> GetAllAsync(int userId, int pageNumber, int pageSize)
        {
            if (userId <= 0)
                return new ApiResponse<List<RFQResponseDto>> { Success = false, Message = "User ID is missing or invalid." };

            var user = await _context.User.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return new ApiResponse<List<RFQResponseDto>> { Success = false, Message = "User ID not found." };

            string roleName = user.Role.RoleName;

            if (roleName == "User")
                return new ApiResponse<List<RFQResponseDto>> { Success = false, Message = "You do not have permission to view RFQs." };

            IQueryable<RFQ> query = _context.RFQ.AsQueryable();

            if (roleName == "Manager")
            {
                var employee = await _context.Employee.FirstOrDefaultAsync(e => e.EmployeeId == user.MasterId);
                if (employee == null)
                {
                    return new ApiResponse<List<RFQResponseDto>> { Success = false, Message = "Manager employee record not found." };
                }

                int managerDeptId = employee.DepartmentId;

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
            else if (roleName != "CFO")
            {
                return new ApiResponse<List<RFQResponseDto>> { Success = false, Message = "Invalid Role." };
            }

            int totalRecords = await query.CountAsync();

            var rfqs = await query.OrderByDescending(r => r.RFQId)
                                  .Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToListAsync();

            var rfqDtos = _mapper.Map<List<RFQResponseDto>>(rfqs);

            return new ApiResponse<List<RFQResponseDto>>
            {
                Success = true,
                Message = "RFQs fetched successfully",
                Data = rfqDtos,
                TotalNumberRecord = totalRecords
            };
        }

        public async Task<ApiResponse<RFQResponseDto>> GetByIdAsync(int id)
        {
            var rfq = await _context.RFQ.FindAsync(id);

            if (rfq == null)
            {
                return new ApiResponse<RFQResponseDto> { Success = false, Message = "RFQ ID not found." };
            }

            var rfqDto = _mapper.Map<RFQResponseDto>(rfq);
            return new ApiResponse<RFQResponseDto> { Success = true, Data = rfqDto, TotalNumberRecord = 1 };
        }

        public async Task<ApiResponse<RFQResponseDto>> UpdateAsync(int id, RFQUpdateDto dto)
        {
            var rfq = await _context.RFQ.FindAsync(id);

            if (rfq == null)
            {
                return new ApiResponse<RFQResponseDto> { Success = false, Message = "RFQ ID not found." };
            }

            if (rfq.Status == "Open")
            {
                return new ApiResponse<RFQResponseDto> { Success = false, Message = "Cannot update RFQ once status is Open." };
            }

            if (await _context.RFQ.AnyAsync(r => r.RFQNumber == dto.RFQNumber && r.RFQId != id))
            {
                return new ApiResponse<RFQResponseDto> { Success = false, Message = "RFQ Number already exists for another record." };
            }

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

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            // Update: Switch to FirstOrDefaultAsync and Include nested properties
            var rfq = await _context.RFQ
                .Include(r => r.RFQVendors)
                    .ThenInclude(v => v.Quotations)
                .Include(r => r.RFQItems)
                    .ThenInclude(i => i.QuotationItems)
                .FirstOrDefaultAsync(r => r.RFQId == id);

            if (rfq == null)
            {
                return new ApiResponse<bool> { Success = false, Message = "RFQ ID not found.", Data = false };
            }

            if (rfq.Status == "Open")
            {
                return new ApiResponse<bool> { Success = false, Message = "Cannot delete RFQ once status is Open.", Data = false };
            }

            // 1. Clear out restricted QuotationItems
            if (rfq.RFQItems != null)
            {
                foreach (var item in rfq.RFQItems)
                {
                    if (item.QuotationItems != null && item.QuotationItems.Any())
                    {
                        _context.QuotationItem.RemoveRange(item.QuotationItems);
                    }
                }
            }

            // 2. Clear out restricted Quotations
            if (rfq.RFQVendors != null)
            {
                foreach (var vendor in rfq.RFQVendors)
                {
                    if (vendor.Quotations != null && vendor.Quotations.Any())
                    {
                        _context.Quotation.RemoveRange(vendor.Quotations);
                    }
                }
            }

            // 3. Delete RFQItems and Vendors (as they were done previously)
            if (rfq.RFQItems != null && rfq.RFQItems.Any())
            {
                _context.RFQItem.RemoveRange(rfq.RFQItems);
            }

            if (rfq.RFQVendors != null && rfq.RFQVendors.Any())
            {
                _context.RFQVendor.RemoveRange(rfq.RFQVendors);
            }

            _context.RFQ.Remove(rfq);
            await _context.SaveChangesAsync();

            return new ApiResponse<bool> { Success = true, Message = "RFQ, Items, and Vendors deleted successfully.", Data = true };
        }
    }
}