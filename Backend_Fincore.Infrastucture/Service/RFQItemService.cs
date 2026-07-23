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

        public async Task<ApiResponse<RFQItemResponseDto>> CreateAsync(RFQItemCreateDto dto)
        {
            if (await _context.RFQItem.AnyAsync(x => x.RFQId == dto.RFQId && x.Name == dto.Name))
            {
                return new ApiResponse<RFQItemResponseDto> { Success = false, Message = "An item with this Name already exists in this RFQ." };
            }

            var rfqItem = new RFQItem
            {
                RFQId = dto.RFQId,
                Name = dto.Name,
                Quantity = dto.Quantity,
                Description = dto.Description
            };

            _context.RFQItem.Add(rfqItem);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<RFQItemResponseDto>(rfqItem);
            return new ApiResponse<RFQItemResponseDto> { Success = true, Message = "RFQ Item added successfully", Data = responseDto, TotalNumberRecord = 1 };
        }

        public async Task<ApiResponse<List<RFQItemResponseDto>>> GetByRfqIdAsync(int rfqId, int pageNumber, int pageSize)
        {
            var query = _context.RFQItem.Where(x => x.RFQId == rfqId);
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

        public async Task<ApiResponse<RFQItemResponseDto>> UpdateAsync(int id, RFQItemUpdateDto dto)
        {
            var rfqItem = await _context.RFQItem.FindAsync(id);

            if (rfqItem == null)
            {
                return new ApiResponse<RFQItemResponseDto> { Success = false, Message = "RFQ Item ID not found." };
            }

            if (await _context.RFQItem.AnyAsync(x => x.RFQId == rfqItem.RFQId && x.Name == dto.Name && x.RFQItemId != id))
            {
                return new ApiResponse<RFQItemResponseDto> { Success = false, Message = "Another item with this Name already exists in this RFQ." };
            }

            rfqItem.Name = dto.Name;
            rfqItem.Quantity = dto.Quantity;
            rfqItem.Description = dto.Description;

            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<RFQItemResponseDto>(rfqItem);
            return new ApiResponse<RFQItemResponseDto> { Success = true, Message = "RFQ Item updated successfully", Data = responseDto, TotalNumberRecord = 1 };
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var rfqItem = await _context.RFQItem.FindAsync(id);

            if (rfqItem == null)
            {
                return new ApiResponse<bool> { Success = false, Message = "RFQ Item ID not found.", Data = false };
            }

            _context.RFQItem.Remove(rfqItem);
            await _context.SaveChangesAsync();

            return new ApiResponse<bool> { Success = true, Message = "RFQ Item deleted successfully.", Data = true };
        }
    }
}