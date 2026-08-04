using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.RFQItem;
using Backend_Fincore.Application.Interface;
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
    public class RFQItemService : IRFQItemService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly ICurrentUserService current;
        private readonly IMemoryCache cache;

        public RFQItemService(AppDbContext db, IMapper mapper, ICurrentUserService current, IMemoryCache cache)
        {
            this.db = db;
            this.mapper = mapper;
            this.current = current;
            this.cache = cache;
        }

        public async Task CreateAsync(RFQItemCreateDto dto)
        {
            if (await db.RFQItem.AnyAsync(x => x.RFQId == dto.RFQId && x.Name == dto.Name && x.IsActive == 1))
            {
                throw new Exception("An active item with this Name already exists in this RFQ.");
            }

            var rfqItem = new RFQItem
            {
                RFQId = dto.RFQId,
                Name = dto.Name,
                Quantity = dto.Quantity,
                Description = dto.Description,

                CreatedBy = current.UserId,
                CreatedAt = DateTime.UtcNow,
                IsActive = 1
            };

            await db.RFQItem.AddAsync(rfqItem);
            await db.SaveChangesAsync();
        }

        public async Task<int> GetCountByRfqIdAsync(int rfqId)
        {
            string cacheKey = $"RFQItem_TotalCount_RFQ{rfqId}";

            if (cache.TryGetValue(cacheKey, out int cachedCount))
            {
                return cachedCount;
            }

            int count = await db.RFQItem.Where(x => x.RFQId == rfqId && x.IsActive == 1).CountAsync();

            cache.Set(cacheKey, count, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30)));

            return count;
        }

        public async Task<List<RFQItemResponseDto>> GetByRfqIdAsync(int rfqId, PaginationDTO pagination)
        {
            string cacheKey = $"RFQItem_List_RFQ{rfqId}_P{pagination.PageNumber}_S{pagination.PageSize}_Search{pagination.Search ?? "none"}";

            if (cache.TryGetValue(cacheKey, out List<RFQItemResponseDto> cachedData))
            {
                return cachedData;
            }

            var query = db.RFQItem.Where(x => x.RFQId == rfqId && x.IsActive == 1);

            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                query = query.Where(x => x.Name.Contains(pagination.Search));
            }

            var items = await query.OrderByDescending(x => x.RFQItemId)
                                   .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                                   .Take(pagination.PageSize)
                                   .ToListAsync();

            var responseData = mapper.Map<List<RFQItemResponseDto>>(items);

            cache.Set(cacheKey, responseData, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30)));

            return responseData;
        }

        public async Task UpdateAsync(int id, RFQItemUpdateDto dto)
        {
            var rfqItem = await db.RFQItem.FirstOrDefaultAsync(x => x.RFQItemId == id && x.IsActive == 1);

            if (rfqItem == null)
            {
                throw new Exception("RFQ Item ID not found or has been deleted.");
            }

            if (await db.RFQItem.AnyAsync(x => x.RFQId == rfqItem.RFQId && x.Name == dto.Name && x.RFQItemId != id && x.IsActive == 1))
            {
                throw new Exception("Another active item with this Name already exists in this RFQ.");
            }

            rfqItem.Name = dto.Name;
            rfqItem.Quantity = dto.Quantity;
            rfqItem.Description = dto.Description;

            rfqItem.ModifiedBy = current.UserId;
            rfqItem.ModifiedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var rfqItem = await db.RFQItem.FirstOrDefaultAsync(x => x.RFQItemId == id && x.IsActive == 1);

            if (rfqItem == null)
            {
                throw new Exception("RFQ Item ID not found or already deleted.");
            }

            rfqItem.IsActive = 0;
            rfqItem.ModifiedBy = current.UserId;
            rfqItem.ModifiedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();
        }
    }
}