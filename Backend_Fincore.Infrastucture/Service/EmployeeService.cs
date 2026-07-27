using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_Fincore.Infrastucture.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly ICurrentUserService currentUser;
        private readonly IMemoryCache cache;

        private const string CacheKeyList = "Cache_Employee_List_";
        private const string CacheKeySingle = "Cache_Employee_Id_";
        private const string CacheKeyCount = "Cache_Employee_Count_";

        public EmployeeService(
            AppDbContext db,
            IMapper mapper,
            ICurrentUserService currentUser,
            IMemoryCache cache)
        {
            this.db = db;
            this.mapper = mapper;
            this.currentUser = currentUser;
            this.cache = cache;
        }

        public async Task<EmployeeReadDTO> AddEmp(EmployeeWriteDTO e)
        {
            var data = mapper.Map<Employee>(e);
            data.IsActive = 1;
            data.CreatedAt = DateTime.UtcNow;
            data.CreatedBy = currentUser.UserId;

            await db.Employee.AddAsync(data);
            await db.SaveChangesAsync();

            var mdata = await db.Employee
                .AsNoTracking()
                .Include(x => x.Company)
                .Include(x => x.Department)
                .Include(x => x.ReportingManager)
                .FirstOrDefaultAsync(x => x.EmployeeId == data.EmployeeId);

            ClearEmployeeCache();

            return mapper.Map<EmployeeReadDTO>(mdata);
        }

        public async Task<bool> delete(int id)
        {
            var data = await db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == id && x.IsActive == 1);

            if (data == null)
            {
                return false;
            }

            // Soft Delete
            data.IsActive = 0;
            data.ModifiedAt = DateTime.UtcNow;
            data.ModifiedBy = currentUser.UserId;

            await db.SaveChangesAsync();

            ClearEmployeeCache();

            return true;
        }

        public async Task<List<EmployeeReadDTO>> GetAll(PaginationDTO pagination)
        {
            string cacheKey = $"{CacheKeyList}Page_{pagination.PageNumber}_Size_{pagination.PageSize}_Search_{pagination.Search ?? "None"}";

            if (cache.TryGetValue(cacheKey, out List<EmployeeReadDTO>? cachedList) && cachedList != null)
            {
                return cachedList;
            }

            var search = db.Employee
                .AsNoTracking()
                .Include(x => x.Company)
                .Include(x => x.Department)
                .Include(x => x.ReportingManager)
                .Where(x => x.IsActive == 1)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pagination.Search))
            {
                search = search.Where(x =>
                    x.FirstName.Contains(pagination.Search) ||
                    x.LastName.Contains(pagination.Search) ||
                    x.EmployeeCode.Contains(pagination.Search) ||
                    x.Company.CompanyName.Contains(pagination.Search) ||
                    x.Department.DepartmentName.Contains(pagination.Search)
                );
            }

            var data = await search
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var result = mapper.Map<List<EmployeeReadDTO>>(data);

            cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

            return result;
        }

        public async Task<EmployeeReadDTO> GetById(int id)
        {
            string cacheKey = $"{CacheKeySingle}{id}";

            if (cache.TryGetValue(cacheKey, out EmployeeReadDTO? cachedItem) && cachedItem != null)
            {
                return cachedItem;
            }

            var data = await db.Employee
                .AsNoTracking()
                .Include(x => x.Company)
                .Include(x => x.Department)
                .Include(x => x.ReportingManager)
                .FirstOrDefaultAsync(x => x.EmployeeId == id && x.IsActive == 1);

            if (data == null)
            {
                return null;
            }

            var result = mapper.Map<EmployeeReadDTO>(data);

            cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

            return result;
        }

        public async Task<int> GetTotalEmployeeRecords(string? search)
        {
            string cacheKey = $"{CacheKeyCount}Search_{search ?? "None"}";

            if (cache.TryGetValue(cacheKey, out int cachedCount))
            {
                return cachedCount;
            }

            var query = db.Employee
                .AsNoTracking()
                .Include(x => x.Company)
                .Include(x => x.Department)
                .Where(x => x.IsActive == 1)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x =>
                    x.FirstName.Contains(search) ||
                    x.LastName.Contains(search) ||
                    x.EmployeeCode.Contains(search) ||
                    x.Company.CompanyName.Contains(search) ||
                    x.Department.DepartmentName.Contains(search)
                );
            }

            int count = await query.CountAsync();

            cache.Set(cacheKey, count, TimeSpan.FromMinutes(10));

            return count;
        }

        public async Task<bool> update(int id, EmployeeWriteDTO e)
        {
            var data = await db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == id && x.IsActive == 1);

            if (data == null)
            {
                return false;
            }

            mapper.Map(e, data);
            data.ModifiedAt = DateTime.UtcNow;
            data.ModifiedBy = currentUser.UserId;

            await db.SaveChangesAsync();

            ClearEmployeeCache();

            return true;
        }

        private void ClearEmployeeCache()
        {
            if (cache is MemoryCache memoryCache)
            {
                memoryCache.Compact(1.0);
            }
        }
    }
}