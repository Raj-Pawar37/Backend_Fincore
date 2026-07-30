using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.RFQ;
using Backend_Fincore.Application.Interface; // Ensure this points to ICurrentUserService
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Caching.Memory;

namespace Backend_Fincore.Infrastucture.Service
{
    public class RFQService : IRFQService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _current;
        private readonly IMemoryCache _cache;

        public RFQService(AppDbContext context, IMapper mapper, ICurrentUserService current, IMemoryCache cache)
        {
            _context = context;
            _mapper = mapper;
            _current = current;
            _cache = cache;
        }

        public async Task CreateAsync(RFQCreateDto dto)
        {
            if (await _context.RFQ.AnyAsync(r => r.RFQNumber == dto.RFQNumber && r.IsActive == 1))
            {
                throw new Exception("RFQ Number already exists.");
            }

            if (!await _context.PurchaseRequisition.AnyAsync(pr => pr.PRId == dto.PRId && pr.IsActive == 1))
            {
                throw new Exception("Purchase Requisition ID not found or is inactive.");
            }

            if (await _context.RFQ.AnyAsync(r => r.PRId == dto.PRId && r.IsActive == 1))
            {
                throw new Exception("An active RFQ already exists for this Purchase Requisition.");
            }

            var rfq = new RFQ
            {
                RFQNumber = dto.RFQNumber,
                Title = dto.Title,
                Description = dto.Description,
                IssueDate = dto.IssueDate,
                ClosingDate = dto.ClosingDate,
                PRId = dto.PRId,
                Status = "Pending",

                CreatedBy = _current.UserId,
                CreatedAt = DateTime.UtcNow,
                IsActive = 1
            };

            await _context.RFQ.AddAsync(rfq);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetCountAsync()
        {
            string cacheKey = "RFQ_TotalCount";

            if (_cache.TryGetValue(cacheKey, out int cachedCount))
            {
                return cachedCount;
            }

            int count = await _context.RFQ.Where(r => r.IsActive == 1).CountAsync();

            _cache.Set(cacheKey, count, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30)));

            return count;
        }

        public async Task<List<RFQResponseDto>> GetAllAsync(PaginationDTO pagination)
        {
            string cacheKey = $"RFQ_List_P{pagination.PageNumber}_S{pagination.PageSize}_Search{pagination.Search ?? "none"}";

            if (_cache.TryGetValue(cacheKey, out List<RFQResponseDto> cachedData))
            {
                return cachedData;
            }

            var user = await _context.User.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == _current.UserId);

            if (user == null) throw new Exception("User not found.");
            if (user.Role == null) throw new Exception("Role not Exist.");

            string roleName = user.Role.RoleName;

            if (roleName == "User" || roleName == "Employee")
            {
                throw new Exception("You are not authorized to view RFQs.");
            }

            IQueryable<RFQ> query = _context.RFQ.Where(r => r.IsActive == 1);

            if (roleName == "Manager" || roleName == "Senior Manager" || roleName == "HOD")
            {
                var employee = await _context.Employee.FirstOrDefaultAsync(e => e.EmployeeId == user.MasterId);
                if (employee == null) throw new Exception("Employee record not found.");

                int managerDeptId = employee.DepartmentId;

                query = query.Where(rfq => _context.PurchaseRequisition.Any(pr =>
                                           pr.PRId == rfq.PRId && pr.IsActive == 1 &&
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
                throw new Exception("Invalid Role.");
            }

            // Apply search from PaginationDTO
            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                query = query.Where(x => x.RFQNumber.Contains(pagination.Search) ||
                                         x.Title.Contains(pagination.Search) ||
                                         x.Status.Contains(pagination.Search));
            }

            var rfqs = await query.OrderByDescending(r => r.CreatedAt)
                                  .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                                  .Take(pagination.PageSize)
                                  .ToListAsync();

            var responseData = _mapper.Map<List<RFQResponseDto>>(rfqs);

            _cache.Set(cacheKey, responseData, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30)));

            return responseData;
        }

        public async Task<RFQResponseDto> GetByIdAsync(int id)
        {
            string cacheKey = $"RFQ_GetById_{id}";

            if (_cache.TryGetValue(cacheKey, out RFQResponseDto cachedData))
            {
                return cachedData;
            }

            var rfq = await _context.RFQ.FirstOrDefaultAsync(r => r.RFQId == id && r.IsActive == 1);

            if (rfq == null)
            {
                return null;
            }

            var responseData = _mapper.Map<RFQResponseDto>(rfq);

            _cache.Set(cacheKey, responseData, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(60)));

            return responseData;
        }

        public async Task UpdateAsync(int id, RFQUpdateDto dto)
        {
            var rfq = await _context.RFQ.FirstOrDefaultAsync(r => r.RFQId == id && r.IsActive == 1);

            if (rfq == null)
            {
                throw new Exception("RFQ not found or has been deleted.");
            }

            if (rfq.Status == "Open")
            {
                throw new Exception("Cannot update RFQ once status is Open.");
            }

            if (await _context.RFQ.AnyAsync(r => r.RFQNumber == dto.RFQNumber && r.RFQId != id && r.IsActive == 1))
            {
                throw new Exception("RFQ Number already exists for another active record.");
            }

            rfq.RFQNumber = dto.RFQNumber;
            rfq.Title = dto.Title;
            rfq.Description = dto.Description;
            rfq.IssueDate = dto.IssueDate;
            rfq.ClosingDate = dto.ClosingDate;

            rfq.ModifiedBy = _current.UserId;
            rfq.ModifiedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(dto.Status))
            {
                rfq.Status = dto.Status;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var rfq = await _context.RFQ
                .Include(r => r.RFQVendors).ThenInclude(v => v.Quotations)
                .Include(r => r.RFQItems).ThenInclude(i => i.QuotationItems)
                .FirstOrDefaultAsync(r => r.RFQId == id && r.IsActive == 1);

            if (rfq == null)
            {
                throw new Exception("RFQ not found or already deleted.");
            }

            if (rfq.Status == "Open")
            {
                throw new Exception("Cannot delete RFQ once status is Open.");
            }

            // Soft delete RFQ
            rfq.IsActive = 0;
            rfq.ModifiedBy = _current.UserId;
            rfq.ModifiedAt = DateTime.UtcNow;

            // Soft delete nested items
            if (rfq.RFQItems != null)
            {
                foreach (var item in rfq.RFQItems)
                {
                    item.IsActive = 0;
                    item.ModifiedBy = _current.UserId;
                    item.ModifiedAt = DateTime.UtcNow;

                    if (item.QuotationItems != null && item.QuotationItems.Any())
                    {
                        foreach (var quotationItem in item.QuotationItems)
                        {
                            quotationItem.IsActive = 0;
                            quotationItem.ModifiedBy = _current.UserId;
                            quotationItem.ModifiedAt = DateTime.UtcNow;
                        }
                    }
                }
            }

            // Soft delete nested vendors
            if (rfq.RFQVendors != null)
            {
                foreach (var vendor in rfq.RFQVendors)
                {
                    vendor.IsActive = 0;
                    vendor.ModifiedBy = _current.UserId;
                    vendor.ModifiedAt = DateTime.UtcNow;

                    if (vendor.Quotations != null && vendor.Quotations.Any())
                    {
                        foreach (var quotation in vendor.Quotations)
                        {
                            quotation.IsActive = 0;
                            quotation.ModifiedBy = _current.UserId;
                            quotation.ModifiedAt = DateTime.UtcNow;
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<RFQDropdownDto>> GetDropdownAsync(string? searchText, int? vendorId, string? status)
        {
            IQueryable<RFQ> query = _context.RFQ.AsNoTracking().Where(x => x.IsActive == 1);

            // Filter RFQs by vendor when VendorId is supplied
            if (vendorId.HasValue && vendorId.Value > 0)
            {
                query = query.Where(rfq => rfq.RFQVendors.Any(rfqVendor => rfqVendor.VendorId == vendorId.Value && rfqVendor.IsActive == 1));
            }

            // Filter by status when supplied
            if (!string.IsNullOrWhiteSpace(status))
            {
                string normalizedStatus = status.Trim();
                query = query.Where(x => x.Status == normalizedStatus);
            }

            // Search by RFQ number or title
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                string search = searchText.Trim();
                query = query.Where(x => x.RFQNumber.Contains(search) || x.Title.Contains(search));
            }

            var data =  await query.OrderByDescending(x => x.CreatedAt).Take(20)
                .Select(x => new RFQDropdownDto
                {
                    RFQId = x.RFQId,
                    RFQNumber = x.RFQNumber,
                    Title = x.Title,
                    Status = x.Status
                }).ToListAsync();

            return data;
        }
    }
}