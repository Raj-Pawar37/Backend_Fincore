using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Vendor;
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
    public class VendorService : IVendorService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly ICurrentUserService currentUser;
        private readonly IMemoryCache cache;

        private const string CacheKeyList = "Cache_Vendor_List_";
        private const string CacheKeySingle = "Cache_Vendor_Id_";
        private const string CacheKeyCount = "Cache_Vendor_Count_";

        public VendorService(
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

        public async Task<VendorReadDTO> AddVendor(VendorWriteDTO v)
        {
            var data = mapper.Map<Vendor>(v);
            data.IsActive = 1;
            data.CreatedBy = currentUser.UserId;
            data.CreatedAt = DateTime.Now;

            await db.Vendor.AddAsync(data);
            await db.SaveChangesAsync();

            var mdata = await db.Vendor
                .Include(x => x.Company)
                .FirstOrDefaultAsync(x => x.VendorId == data.VendorId);

            ClearVendorCache();

            return mapper.Map<VendorReadDTO>(mdata);
        }

        public async Task<List<VendorReadDTO>> GetAll(PaginationDTO pagination)
        {
            string cacheKey = $"{CacheKeyList}Page_{pagination.PageNumber}_Size_{pagination.PageSize}_Search_{pagination.Search ?? "None"}";

            if (cache.TryGetValue(cacheKey, out List<VendorReadDTO>? cachedList) && cachedList != null)
            {
                return cachedList;
            }

            var search = db.Vendor
                .Include(x => x.Company)
                .Where(x => x.IsActive == 1)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pagination.Search))
            {
                search = search.Where(x =>
                    x.VendorName.Contains(pagination.Search) ||
                    x.VendorCode.Contains(pagination.Search) ||
                    x.Company.CompanyName.Contains(pagination.Search)
                );
            }

            var data = await search
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var result = mapper.Map<List<VendorReadDTO>>(data);

            cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

            return result;
        }

        public async Task<VendorReadDTO> GetById(int id)
        {
            string cacheKey = $"{CacheKeySingle}{id}";

            if (cache.TryGetValue(cacheKey, out VendorReadDTO? cachedItem) && cachedItem != null)
            {
                return cachedItem;
            }

            var data = await db.Vendor
                .Include(x => x.Company)
                .FirstOrDefaultAsync(x => x.VendorId == id && x.IsActive == 1);

            if (data == null)
            {
                return null;
            }

            var result = mapper.Map<VendorReadDTO>(data);

            cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

            return result;
        }

        public async Task<bool> UpdateVendor(int id, VendorWriteDTO v)
        {
            var data = await db.Vendor
                .FirstOrDefaultAsync(x => x.VendorId == id && x.IsActive == 1);

            if (data == null)
            {
                return false;
            }

            mapper.Map(v, data);
            data.ModifiedBy = currentUser.UserId;
            data.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            ClearVendorCache();

            return true;
        }

        public async Task<bool> DeleteVendor(int id)
        {
            var data = await db.Vendor
                .FirstOrDefaultAsync(x => x.VendorId == id && x.IsActive == 1);

            if (data == null)
            {
                return false;
            }

            bool isUsed =
                await db.RFQVendor.AnyAsync(x => x.VendorId == id) ||
                await db.PurchaseOrder.AnyAsync(x => x.VendorId == id) ||
                await db.APInvoice.AnyAsync(x => x.VendorId == id) ||
                await db.WorkOrder.AnyAsync(x => x.VendorId == id);

            if (isUsed)
            {
                throw new InvalidOperationException("Vendor cannot be deleted because it is associated with other records.");
            }

            // Soft Delete
            data.IsActive = 0;
            data.ModifiedBy = currentUser.UserId;
            data.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            ClearVendorCache();

            return true;
        }

        public async Task<int> GetTotalVendorRecord(string? search)
        {
            string cacheKey = $"{CacheKeyCount}Search_{search ?? "None"}";

            if (cache.TryGetValue(cacheKey, out int cachedCount))
            {
                return cachedCount;
            }

            var data = db.Vendor
                .Include(x => x.Company)
                .Where(x => x.IsActive == 1)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                data = data.Where(x =>
                    x.VendorName.Contains(search) ||
                    x.VendorCode.Contains(search) ||
                    x.Company.CompanyName.Contains(search));
            }

            int count = await data.CountAsync();

            cache.Set(cacheKey, count, TimeSpan.FromMinutes(10));

            return count;
        }

        private void ClearVendorCache()
        {
            if (cache is MemoryCache memoryCache)
            {
                memoryCache.Compact(1.0);
            }
        }
    }
}