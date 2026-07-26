using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.RFQVendor;
using Backend_Fincore.Application.Interfaces;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Caching.Memory;

namespace Backend_Fincore.Application.Services
{
    public class RFQVendorService : IRFQVendorService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly ICurrentUserService current;
        private readonly IMemoryCache cache;

        public RFQVendorService(AppDbContext db, IMapper mapper, ICurrentUserService current, IMemoryCache cache)
        {
            this.db = db;
            this.mapper = mapper;
            this.current = current;
            this.cache = cache;
        }

        public async Task CreateAsync(RFQVendorCreateDto dto)
        {
            if (!await db.RFQ.AnyAsync(r => r.RFQId == dto.RFQId && r.IsActive == 1))
            {
                throw new Exception("RFQ ID not found or is inactive.");
            }

            if (!await db.Vendor.AnyAsync(v => v.VendorId == dto.VendorId))
            {
                throw new Exception("Vendor ID not found.");
            }

            if (await db.RFQVendor.AnyAsync(rv => rv.RFQId == dto.RFQId && rv.VendorId == dto.VendorId && rv.IsActive == 1))
            {
                throw new Exception("This Vendor is already actively invited to this RFQ.");
            }

            var rfqVendor = new RFQVendor
            {
                RFQId = dto.RFQId,
                VendorId = dto.VendorId,
                SentDate = dto.SentDate,
                ResponseStatus = "Invited",

                CreatedBy = current.UserId,
                CreatedAt = DateTime.UtcNow,
                IsActive = 1
            };

            await db.RFQVendor.AddAsync(rfqVendor);
            await db.SaveChangesAsync();
        }

        public async Task<int> GetCountByRfqIdAsync(int rfqId)
        {
            string cacheKey = $"RFQVendor_TotalCount_RFQ{rfqId}";

            if (cache.TryGetValue(cacheKey, out int cachedCount))
            {
                return cachedCount;
            }

            int count = await db.RFQVendor.Where(x => x.RFQId == rfqId && x.IsActive == 1).CountAsync();

            cache.Set(cacheKey, count, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30)));

            return count;
        }

        public async Task<List<RFQVendorResponseDto>> GetByRfqIdAsync(int rfqId, PaginationDTO pagination)
        {
            string cacheKey = $"RFQVendor_List_RFQ{rfqId}_P{pagination.PageNumber}_S{pagination.PageSize}_Search{pagination.Search ?? "none"}";

            if (cache.TryGetValue(cacheKey, out List<RFQVendorResponseDto> cachedData))
            {
                return cachedData;
            }

            var query = db.RFQVendor.Where(x => x.RFQId == rfqId && x.IsActive == 1);

            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                query = query.Where(x => x.ResponseStatus.Contains(pagination.Search));
            }

            var vendors = await query.OrderByDescending(x => x.RFQVendorId)
                                     .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                                     .Take(pagination.PageSize)
                                     .ToListAsync();

            var responseData = mapper.Map<List<RFQVendorResponseDto>>(vendors);

            cache.Set(cacheKey, responseData, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30)));

            return responseData;
        }

        public async Task UpdateAsync(int id, RFQVendorUpdateDto dto)
        {
            var rfqVendor = await db.RFQVendor.FirstOrDefaultAsync(x => x.RFQVendorId == id && x.IsActive == 1);

            if (rfqVendor == null)
            {
                throw new Exception("RFQ Vendor mapping ID not found or deleted.");
            }

            if (!await db.RFQ.AnyAsync(r => r.RFQId == dto.RFQId && r.IsActive == 1))
            {
                throw new Exception("RFQ ID not found or inactive.");
            }

            if (!await db.Vendor.AnyAsync(v => v.VendorId == dto.VendorId))
            {
                throw new Exception("Vendor ID not found.");
            }

            rfqVendor.RFQId = dto.RFQId;
            rfqVendor.VendorId = dto.VendorId;

            if (!string.IsNullOrEmpty(dto.ResponseStatus))
            {
                rfqVendor.ResponseStatus = dto.ResponseStatus;
            }
            if (dto.ResponseDate.HasValue)
            {
                rfqVendor.ResponseDate = dto.ResponseDate;
            }

            rfqVendor.ModifiedBy = current.UserId;
            rfqVendor.ModifiedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var rfqVendor = await db.RFQVendor.FirstOrDefaultAsync(x => x.RFQVendorId == id && x.IsActive == 1);

            if (rfqVendor == null)
            {
                throw new Exception("RFQ Vendor mapping ID not found or already deleted.");
            }

            rfqVendor.IsActive = 0;
            rfqVendor.ModifiedBy = current.UserId;
            rfqVendor.ModifiedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();
        }
    }
}