using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.PurchaseRequisition;
using Backend_Fincore.Application.Interface; // ICurrentUserService
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
    public class PurchaseRequisitionService : IPurchaseRequisitionService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _current;
        private readonly IMemoryCache _cache;

        public PurchaseRequisitionService(AppDbContext context, IMapper mapper, ICurrentUserService current, IMemoryCache cache)
        {
            _context = context;
            _mapper = mapper;
            _current = current;
            _cache = cache;
        }

        public async Task<int> GetCountAsync()
        {
            string cacheKey = "PR_TotalCount";

            if (_cache.TryGetValue(cacheKey, out int cachedCount))
            {
                return cachedCount;
            }

            int count = await _context.PurchaseRequisition.CountAsync();

            _cache.Set(cacheKey, count, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30)));

            return count;
        }

        public async Task<List<PurchaseRequisitionResponseDto>> GetAllAsync(PaginationDTO pagination)
        {
            string cacheKey = $"PR_List_P{pagination.PageNumber}_S{pagination.PageSize}_Search{pagination.Search ?? "none"}";

            if (_cache.TryGetValue(cacheKey, out List<PurchaseRequisitionResponseDto> cachedData))
            {
                return cachedData;
            }

            var user = await _context.User
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == _current.UserId);

            if (user == null) throw new Exception("User not found.");
            if (user.Role == null) throw new Exception("Role not Exist.");

            string roleName = user.Role.RoleName;

            if (roleName == "User")
            {
                throw new Exception("You do not have permission to view PRs.");
            }

            IQueryable<PurchaseRequisition> query = _context.PurchaseRequisition.AsQueryable();

            if (roleName == "Manager")
            {
                var employee = await _context.Employee.FirstOrDefaultAsync(e => e.EmployeeId == user.MasterId);

                if (employee == null)
                {
                    throw new Exception("Manager employee record not found.");
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
                throw new Exception("Invalid Role.");
            }

            // Pagination Search Logic
            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                query = query.Where(pr => pr.PRNumber.Contains(pagination.Search) ||
                                          pr.Title.Contains(pagination.Search) ||
                                          pr.Status.Contains(pagination.Search));
            }

            var prs = await query.OrderByDescending(pr => pr.PRId)
                                 .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                                 .Take(pagination.PageSize)
                                 .ToListAsync();

            var responseData = _mapper.Map<List<PurchaseRequisitionResponseDto>>(prs);

            _cache.Set(cacheKey, responseData, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30)));

            return responseData;
        }

        public async Task<PurchaseRequisitionResponseDto> GetByIdAsync(int id)
        {
            string cacheKey = $"PR_GetById_{id}";

            if (_cache.TryGetValue(cacheKey, out PurchaseRequisitionResponseDto cachedData))
            {
                return cachedData;
            }

            var pr = await _context.PurchaseRequisition.FindAsync(id);

            if (pr == null)
            {
                return null;
            }

            var responseData = _mapper.Map<PurchaseRequisitionResponseDto>(pr);

            _cache.Set(cacheKey, responseData, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(60)));

            return responseData;
        }

        public async Task UpdateAsync(int id, PurchaseRequisitionUpdateDto dto)
        {
            var pr = await _context.PurchaseRequisition.FindAsync(id);

            if (pr == null)
            {
                throw new Exception("Purchase Requisition not found.");
            }

            if (!string.IsNullOrEmpty(dto.Title)) pr.Title = dto.Title;
            if (!string.IsNullOrEmpty(dto.PRNumber)) pr.PRNumber = dto.PRNumber;
            if (!string.IsNullOrEmpty(dto.Description)) pr.Description = dto.Description;
            if (!string.IsNullOrEmpty(dto.Status)) pr.Status = dto.Status;

            pr.ModifiedBy = _current.UserId;
            pr.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<List<PRDropdownResponseDto>> GetPRDropdownAsync(string? searchText, int? departmentId)
        {
            string cacheKey = $"PR_Dropdown_{searchText ?? "none"}_{departmentId ?? 0}";

            if (_cache.TryGetValue(cacheKey, out List<PRDropdownResponseDto> cachedData))
            {
                return cachedData;
            }

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
                             .Take(20); // Limit dropdown results for performance
            }

            var prs = await query.Select(pr => new PRDropdownResponseDto
            {
                PRId = pr.PRId,
                PRNumber = pr.PRNumber,
                Title = pr.Title
            }).ToListAsync();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));

            _cache.Set(cacheKey, prs, cacheOptions);

            return prs;
        }
    }
}