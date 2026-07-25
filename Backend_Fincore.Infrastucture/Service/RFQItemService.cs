using AutoMapper;
using Backend_Fincore.Application.DTOs.RFQItem;
using Backend_Fincore.Application.Interfaces;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Backend_Fincore.Response;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Backend_Fincore.Application.Services
{
    public class RFQItemService : IRFQItemService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public RFQItemService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<RFQItemResponseDto>> CreateAsync(RFQItemCreateDto dto, int userId)
        {
            if (await _context.RFQItem.AnyAsync(x => x.RFQId == dto.RFQId && x.Name == dto.Name && x.IsActive == 1))
            {
                return new ApiResponse<RFQItemResponseDto> { Success = false, Message = "An active item with this Name already exists in this RFQ." };
            }

            var rfqItem = new RFQItem
            {
                RFQId = dto.RFQId,
                Name = dto.Name,
                Quantity = dto.Quantity,
                Description = dto.Description,

                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = 1
            };

            _context.RFQItem.Add(rfqItem);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<RFQItemResponseDto>(rfqItem);
            return new ApiResponse<RFQItemResponseDto> { Success = true, Message = "RFQ Item added successfully", Data = responseDto, TotalNumberRecord = 1 };
        }

        public async Task<ApiResponse<List<RFQItemResponseDto>>> GetByRfqIdAsync(int rfqId, int pageNumber, int pageSize)
        {
            // FIXED: Added == 1 here
            var query = _context.RFQItem.Where(x => x.RFQId == rfqId && x.IsActive == 1);
            int totalRecords = await query.CountAsync();

            var items = await query.OrderByDescending(x => x.RFQItemId)
                                   .Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            var itemDtos = _mapper.Map<List<RFQItemResponseDto>>(items);

            return new ApiResponse<List<RFQItemResponseDto>>
            {
                Success = true,
                Message = "RFQ Items fetched successfully",
                Data = itemDtos,
                TotalNumberRecord = totalRecords
            };
        }

        public async Task<ApiResponse<RFQItemResponseDto>> UpdateAsync(int id, RFQItemUpdateDto dto, int userId)
        {
            // FIXED: Added == 1 here
            var rfqItem = await _context.RFQItem.FirstOrDefaultAsync(x => x.RFQItemId == id && x.IsActive == 1);

            if (rfqItem == null)
            {
                return new ApiResponse<RFQItemResponseDto> { Success = false, Message = "RFQ Item ID not found or has been deleted." };
            }

            // FIXED: Added == 1 here
            if (await _context.RFQItem.AnyAsync(x => x.RFQId == rfqItem.RFQId && x.Name == dto.Name && x.RFQItemId != id && x.IsActive == 1))
            {
                return new ApiResponse<RFQItemResponseDto> { Success = false, Message = "Another active item with this Name already exists in this RFQ." };
            }

            rfqItem.Name = dto.Name;
            rfqItem.Quantity = dto.Quantity;
            rfqItem.Description = dto.Description;

            rfqItem.ModifiedBy = userId;
            rfqItem.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<RFQItemResponseDto>(rfqItem);
            return new ApiResponse<RFQItemResponseDto> { Success = true, Message = "RFQ Item updated successfully", Data = responseDto, TotalNumberRecord = 1 };
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id, int userId)
        {
            var rfqItem = await _context.RFQItem.FirstOrDefaultAsync(x => x.RFQItemId == id && x.IsActive == 1);

            if (rfqItem == null)
            {
                return new ApiResponse<bool> { Success = false, Message = "RFQ Item ID not found or already deleted.", Data = false };
            }

            // Soft delete logic
            rfqItem.IsActive = 0;
            rfqItem.ModifiedBy = userId;
            rfqItem.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ApiResponse<bool> { Success = true, Message = "RFQ Item deleted successfully.", Data = true };
        }
    }
}