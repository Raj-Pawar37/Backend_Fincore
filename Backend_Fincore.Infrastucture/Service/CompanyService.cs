using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Company;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_Fincore.Infrastucture.Service
{
    public class CompanyService : ICompanyService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly ICurrentUserService currentUser;
        private readonly IMemoryCache cache;

        private const string CacheKeyList = "Cache_Company_List_";
        private const string CacheKeyDropdown = "Cache_Company_Dropdown_";
        private const string CacheKeySingle = "Cache_Company_Id_";
        private const string CacheKeyCount = "Cache_Company_Count_";

        public CompanyService(
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

        public async Task<CompanyReadDTO> AddCompany(CompanyWriteDTO c)
        {
            var data = mapper.Map<Company>(c);
            data.IsActive = 1;
            data.CreatedAt = DateTime.Now;
            data.CreatedBy = currentUser.UserId;

            await db.Company.AddAsync(data);
            await db.SaveChangesAsync();

            var mdata = await db.Company
                .AsNoTracking()
                .Include(x => x.Country)
                .Include(x => x.State)
                .Include(x => x.City)
                .FirstOrDefaultAsync(x => x.CompanyId == data.CompanyId);

            ClearCompanyCache();

            return mapper.Map<CompanyReadDTO>(mdata);
        }

        public async Task<bool> DeleteCompany(int id)
        {
            var company = await db.Company
                .FirstOrDefaultAsync(x => x.CompanyId == id && x.IsActive == 1);

            if (company == null)
                return false;

            bool hasCustomers = await db.Customer
                .AnyAsync(x => x.CompanyId == id && x.IsActive == 1);

            if (hasCustomers)
            {
                throw new InvalidOperationException("Company cannot be deleted because it has customer records.");
            }

            // Soft Delete
            company.IsActive = 0;
            company.ModifiedBy = currentUser.UserId;
            company.ModifiedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            ClearCompanyCache();

            return true;
        }

        public async Task<List<CompanyReadDTO>> GetAll(PaginationDTO pagination)
        {
            string cacheKey = $"{CacheKeyList}Page_{pagination.PageNumber}_Size_{pagination.PageSize}_Search_{pagination.Search ?? "None"}";

            if (cache.TryGetValue(cacheKey, out List<CompanyReadDTO>? cachedList) && cachedList != null)
            {
                return cachedList;
            }

            var search = db.Company
                .AsNoTracking()
                .Include(x => x.Country)
                .Include(x => x.State)
                .Include(x => x.City)
                .Where(x => x.IsActive == 1)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pagination.Search))
            {
                search = search.Where(x =>
                    x.CompanyName.Contains(pagination.Search) ||
                    x.CompanyCode.Contains(pagination.Search)
                );
            }

            var data = await search
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var result = mapper.Map<List<CompanyReadDTO>>(data);

            cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

            return result;
        }

        public async Task<CompanyReadDTO> GetById(int id)
        {
            string cacheKey = $"{CacheKeySingle}{id}";

            if (cache.TryGetValue(cacheKey, out CompanyReadDTO? cachedItem) && cachedItem != null)
            {
                return cachedItem;
            }

            var gid = await db.Company
                .AsNoTracking()
                .Include(x => x.Country)
                .Include(x => x.State)
                .Include(x => x.City)
                .FirstOrDefaultAsync(x => x.CompanyId == id && x.IsActive == 1);

            if (gid == null)
            {
                return null;
            }

            var result = mapper.Map<CompanyReadDTO>(gid);

            cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

            return result;
        }

        public async Task<int> GetTotalCompanyRecords(string? search)
        {
            string cacheKey = $"{CacheKeyCount}Search_{search ?? "None"}";

            if (cache.TryGetValue(cacheKey, out int cachedCount))
            {
                return cachedCount;
            }

            var data = db.Company
                .AsNoTracking()
                .Where(x => x.IsActive == 1)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                data = data.Where(x =>
                    x.CompanyName.Contains(search) ||
                    x.CompanyCode.Contains(search)
                );
            }

            int count = await data.CountAsync();

            cache.Set(cacheKey, count, TimeSpan.FromMinutes(10));

            return count;
        }

        public async Task<bool> UpdateCompany(int id, CompanyWriteDTO c)
        {
            var data = await db.Company
                .FirstOrDefaultAsync(x => x.CompanyId == id && x.IsActive == 1);

            if (data == null)
            {
                return false;
            }

            mapper.Map(c, data);
            data.ModifiedBy = currentUser.UserId;
            data.ModifiedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            ClearCompanyCache();

            return true;
        }

        public async Task<List<CompanyDropdownDTO>> GetCompanyDropdown(string? search)
        {
            string cacheKey = $"{CacheKeyDropdown}{search ?? "All"}";

            if (cache.TryGetValue(cacheKey, out List<CompanyDropdownDTO>? cachedDropdown) && cachedDropdown != null)
            {
                return cachedDropdown;
            }

            var companies = await db.Company
                .Where(x => x.IsActive == 1 &&
                       (string.IsNullOrEmpty(search) ||
                        x.CompanyName.Contains(search)))
                .ToListAsync();

            var result = mapper.Map<List<CompanyDropdownDTO>>(companies);

            cache.Set(cacheKey, result, TimeSpan.FromMinutes(15));

            return result;
        }

        private void ClearCompanyCache()
        {
            if (cache is MemoryCache memoryCache)
            {
                memoryCache.Compact(1.0);
            }
        }
    }
}