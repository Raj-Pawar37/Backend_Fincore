using Backend_Fincore.Application.DTOs.RFQVendor;
using Backend_Fincore.Response;

namespace Backend_Fincore.Application.Interfaces
{
    public interface IRFQVendorService
    {
        Task<ApiResponse<RFQVendorResponseDto>> CreateAsync(RFQVendorCreateDto dto);
        Task<ApiResponse<List<RFQVendorResponseDto>>> GetByRfqIdAsync(int rfqId);
        Task<ApiResponse<RFQVendorResponseDto>> UpdateAsync(int id, RFQVendorUpdateDto dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}