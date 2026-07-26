using Backend_Fincore.Application.DTOs.RFQVendor;
using Backend_Fincore.Response;

namespace Backend_Fincore.Application.Interface
{
    public interface IRFQVendorService
    {
        Task<ApiResponse<RFQVendorResponseDto>> CreateAsync(RFQVendorCreateDto dto, int userId);
        Task<ApiResponse<List<RFQVendorResponseDto>>> GetByRfqIdAsync(int rfqId, int pageNumber, int pageSize);
        Task<ApiResponse<RFQVendorResponseDto>> UpdateAsync(int id, RFQVendorUpdateDto dto, int userId);
        Task<ApiResponse<bool>> DeleteAsync(int id);

    }
}