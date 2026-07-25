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
using System;

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

        public async Task<ApiResponse<RFQVendorResponseDto>> CreateAsync(RFQVendorCreateDto dto, int userId)
        {
            // Updated to check r.IsActive == 1
            if (!await _context.RFQ.AnyAsync(r => r.RFQId == dto.RFQId && r.IsActive == 1))
            {
                return new ApiResponse<RFQVendorResponseDto> { Success = false, Message = "RFQ ID not found or is inactive." };
            }

            if (!await _context.Vendor.AnyAsync(v => v.VendorId == dto.VendorId))
            {
                return new ApiResponse<RFQVendorResponseDto> { Success = false, Message = "Vendor ID not found." };
            }

            // Updated to check rv.IsActive == 1
            if (await _context.RFQVendor.AnyAsync(rv => rv.RFQId == dto.RFQId && rv.VendorId == dto.VendorId && rv.IsActive == 1))
            {
                return new ApiResponse<RFQVendorResponseDto> { Success = false, Message = "This Vendor is already actively invited to this RFQ." };
            }

            var rfqVendor = new RFQVendor
            {
                RFQId = dto.RFQId,
                VendorId = dto.VendorId,
                SentDate = dto.SentDate,
                ResponseStatus = "Invited",

                // Audit Trail
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = 1 // Updated to byte value 1
            };

            _context.RFQVendor.Add(rfqVendor);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<RFQVendorResponseDto>(rfqVendor);
            return new ApiResponse<RFQVendorResponseDto> { Success = true, Message = "Vendor added to RFQ successfully", Data = responseDto, TotalNumberRecord = 1 };
        }

        public async Task<ApiResponse<List<RFQVendorResponseDto>>> GetByRfqIdAsync(int rfqId, int pageNumber, int pageSize)
        {
            // Only fetch active records (Updated to == 1)
            var query = _context.RFQVendor.Where(x => x.RFQId == rfqId && x.IsActive == 1);
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

        public async Task<ApiResponse<RFQVendorResponseDto>> UpdateAsync(int id, RFQVendorUpdateDto dto, int userId)
        {
            // Updated to FirstOrDefaultAsync with IsActive == 1
            var rfqVendor = await _context.RFQVendor.FirstOrDefaultAsync(x => x.RFQVendorId == id && x.IsActive == 1);

            if (rfqVendor == null)
            {
                return new ApiResponse<RFQVendorResponseDto> { Success = false, Message = "RFQ Vendor mapping ID not found or deleted." };
            }

            // Updated to check r.IsActive == 1
            if (!await _context.RFQ.AnyAsync(r => r.RFQId == dto.RFQId && r.IsActive == 1))
            {
                return new ApiResponse<RFQVendorResponseDto> { Success = false, Message = "RFQ ID not found or inactive." };
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

            // Audit Trail
            rfqVendor.ModifiedBy = userId;
            rfqVendor.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<RFQVendorResponseDto>(rfqVendor);
            return new ApiResponse<RFQVendorResponseDto> { Success = true, Message = "RFQ Vendor updated successfully", Data = responseDto, TotalNumberRecord = 1 };
        }

        // Updated signature to include int userId
        public async Task<ApiResponse<bool>> DeleteAsync(int id, int userId)
        {
            try
            {
                // Updated to FirstOrDefaultAsync with IsActive == 1
                var rfqVendor = await _context.RFQVendor.FirstOrDefaultAsync(x => x.RFQVendorId == id && x.IsActive == 1);

                if (rfqVendor == null)
                {
                    return new ApiResponse<bool> { Success = false, Message = "RFQ Vendor mapping ID not found or already deleted.", Data = false };
                }

                // Soft delete logic
                rfqVendor.IsActive = 0; // Updated to 0
                rfqVendor.ModifiedBy = userId;
                rfqVendor.ModifiedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return new ApiResponse<bool> { Success = true, Message = "RFQ Vendor removed successfully.", Data = true };
            }
            catch (Exception)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "An error occurred while trying to delete this RFQ Vendor.",
                    Data = false
                };
            }
        }
    }
}