using AutoMapper;
using Backend_Fincore.Application.DTOs.RFQVendor;
using Backend_Fincore.Application.Interfaces;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Backend_Fincore.Response;
using Microsoft.EntityFrameworkCore;

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

        // ==========================================
        // 1. CREATE
        // ==========================================
        public async Task<ApiResponse<RFQVendorResponseDto>> CreateAsync(RFQVendorCreateDto dto)
        {
            // Rule: If RFQ doesnt exist throw error 
            if (!await _context.RFQ.AnyAsync(r => r.RFQId == dto.RFQId))
            {
                return new ApiResponse<RFQVendorResponseDto> { Success = false, Message = "RFQ ID not found." };
            }

            // Rule: If VendorID doesnt exist throw error
            if (!await _context.Vendor.AnyAsync(v => v.VendorId == dto.VendorId))
            {
                return new ApiResponse<RFQVendorResponseDto> { Success = false, Message = "Vendor ID not found." };
            }

            // Extra safety rule: Prevent duplicate vendor invitations for the same RFQ (based on your DB Index)
            if (await _context.RFQVendor.AnyAsync(rv => rv.RFQId == dto.RFQId && rv.VendorId == dto.VendorId))
            {
                return new ApiResponse<RFQVendorResponseDto> { Success = false, Message = "This Vendor is already invited to this RFQ." };
            }

            var rfqVendor = new RFQVendor
            {
                RFQId = dto.RFQId,
                VendorId = dto.VendorId,
                SentDate = dto.SentDate,
                ResponseStatus = "Invited" // DB default behavior
            };

            _context.RFQVendor.Add(rfqVendor);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<RFQVendorResponseDto>(rfqVendor);
            return new ApiResponse<RFQVendorResponseDto> { Success = true, Message = "Vendor added to RFQ successfully", Data = responseDto, TotalNumberRecord = 1 };
        }

        // ==========================================
        // 2. READ BY RFQ ID
        // ==========================================
        public async Task<ApiResponse<List<RFQVendorResponseDto>>> GetByRfqIdAsync(int rfqId)
        {
            var vendors = await _context.RFQVendor
                                        .Where(x => x.RFQId == rfqId)
                                        .ToListAsync();

            var vendorDtos = _mapper.Map<List<RFQVendorResponseDto>>(vendors);

            return new ApiResponse<List<RFQVendorResponseDto>>
            {
                Success = true,
                Message = "RFQ Vendors fetched successfully",
                Data = vendorDtos,
                TotalNumberRecord = vendorDtos.Count
            };
        }

        // ==========================================
        // 3. UPDATE
        // ==========================================
        public async Task<ApiResponse<RFQVendorResponseDto>> UpdateAsync(int id, RFQVendorUpdateDto dto)
        {
            var rfqVendor = await _context.RFQVendor.FindAsync(id);

            // Rule: If RFQVendorId doesnt found then throw error 
            if (rfqVendor == null)
            {
                return new ApiResponse<RFQVendorResponseDto> { Success = false, Message = "RFQ Vendor mapping ID not found." };
            }

            // Rule: If RFQ doesnt exist throw error 
            if (!await _context.RFQ.AnyAsync(r => r.RFQId == dto.RFQId))
            {
                return new ApiResponse<RFQVendorResponseDto> { Success = false, Message = "RFQ ID not found." };
            }

            // Rule: If VendorID doesnt exist throw error
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

        // ==========================================
        // 4. DELETE
        // ==========================================
        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var rfqVendor = await _context.RFQVendor.FindAsync(id);

            // Rule: If RFQVendorId doesnt found then throw error 
            if (rfqVendor == null)
            {
                return new ApiResponse<bool> { Success = false, Message = "RFQ Vendor mapping ID not found.", Data = false };
            }

            _context.RFQVendor.Remove(rfqVendor);
            await _context.SaveChangesAsync();

            return new ApiResponse<bool> { Success = true, Message = "RFQ Vendor removed successfully.", Data = true };
        }
    }
}