using AutoMapper;
using Backend_Fincore.Application.DTOs.RFQVendor;
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
    public class RFQVendorService : IRFQVendorService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public RFQVendorService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<RFQVendorResponseDto>> CreateAsync(RFQVendorCreateDto dto)
        {
            if (!await _context.RFQ.AnyAsync(r => r.RFQId == dto.RFQId))
            {
                return new ApiResponse<RFQVendorResponseDto> { Success = false, Message = "RFQ ID not found." };
            }

            if (!await _context.Vendor.AnyAsync(v => v.VendorId == dto.VendorId))
            {
                return new ApiResponse<RFQVendorResponseDto> { Success = false, Message = "Vendor ID not found." };
            }

            if (await _context.RFQVendor.AnyAsync(rv => rv.RFQId == dto.RFQId && rv.VendorId == dto.VendorId))
            {
                return new ApiResponse<RFQVendorResponseDto> { Success = false, Message = "This Vendor is already invited to this RFQ." };
            }

            var rfqVendor = new RFQVendor
            {
                RFQId = dto.RFQId,
                VendorId = dto.VendorId,
                SentDate = dto.SentDate,
                ResponseStatus = "Invited"
            };

            _context.RFQVendor.Add(rfqVendor);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<RFQVendorResponseDto>(rfqVendor);
            return new ApiResponse<RFQVendorResponseDto> { Success = true, Message = "Vendor added to RFQ successfully", Data = responseDto, TotalNumberRecord = 1 };
        }

        public async Task<ApiResponse<List<RFQVendorResponseDto>>> GetByRfqIdAsync(int rfqId, int pageNumber, int pageSize)
        {
            var query = _context.RFQVendor.Where(x => x.RFQId == rfqId);
            int totalRecords = await query.CountAsync();

            var vendors = await query.OrderByDescending(x => x.RFQVendorId)
                                     .Skip((pageNumber - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToListAsync();

            var vendorDtos = _mapper.Map<List<RFQVendorResponseDto>>(vendors);

            return new ApiResponse<List<RFQVendorResponseDto>>
            {
                Success = true,
                Message = "RFQ Vendors fetched successfully",
                Data = vendorDtos,
                TotalNumberRecord = totalRecords
            };
        }

        public async Task<ApiResponse<RFQVendorResponseDto>> UpdateAsync(int id, RFQVendorUpdateDto dto)
        {
            var rfqVendor = await _context.RFQVendor.FindAsync(id);

            if (rfqVendor == null)
            {
                return new ApiResponse<RFQVendorResponseDto> { Success = false, Message = "RFQ Vendor mapping ID not found." };
            }

            if (!await _context.RFQ.AnyAsync(r => r.RFQId == dto.RFQId))
            {
                return new ApiResponse<RFQVendorResponseDto> { Success = false, Message = "RFQ ID not found." };
            }

            if (!await _context.Vendor.AnyAsync(v => v.VendorId == dto.VendorId))
            {
                return new ApiResponse<RFQVendorResponseDto> { Success = false, Message = "Vendor ID not found." };
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

            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<RFQVendorResponseDto>(rfqVendor);
            return new ApiResponse<RFQVendorResponseDto> { Success = true, Message = "RFQ Vendor updated successfully", Data = responseDto, TotalNumberRecord = 1 };
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var rfqVendor = await _context.RFQVendor.FindAsync(id);

                if (rfqVendor == null)
                {
                    return new ApiResponse<bool> { Success = false, Message = "RFQ Vendor mapping ID not found.", Data = false };
                }

                // remove only the RFQVendor
                _context.RFQVendor.Remove(rfqVendor);
                await _context.SaveChangesAsync();

                return new ApiResponse<bool> { Success = true, Message = "RFQ Vendor removed successfully.", Data = true };
            }
            catch (Exception)
            {
                // If the database blocks the deletion (e.g., because a Quotation exists),
                // we catch the error here so the server doesn't crash with a 500 error.
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Cannot delete this RFQ Vendor because they have linked records in Quotation ",
                    Data = false
                };
            }
        }
    }
}