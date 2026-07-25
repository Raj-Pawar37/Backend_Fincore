using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.RFQItem;
using Backend_Fincore.Application.Interfaces;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Backend_Fincore.Application.Services
{
    public class RFQItemService : IRFQItemService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly ICurrentUserService current;

        public RFQItemService(AppDbContext db, IMapper mapper, ICurrentUserService current)
        {
            this.db = db;
            this.mapper = mapper;
            this.current = current;
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
            return await db.RFQItem.Where(x => x.RFQId == rfqId && x.IsActive == 1).CountAsync();
        }

        public async Task<List<RFQItemResponseDto>> GetByRfqIdAsync(int rfqId, PaginationDTO pagination)
        {
            var query = db.RFQItem.Where(x => x.RFQId == rfqId && x.IsActive == 1);

            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                query = query.Where(x => x.Name.Contains(pagination.Search));
            }

            var items = await query.OrderByDescending(x => x.RFQItemId)
                                   .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                                   .Take(pagination.PageSize)
                                   .ToListAsync();

            return mapper.Map<List<RFQItemResponseDto>>(items);
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