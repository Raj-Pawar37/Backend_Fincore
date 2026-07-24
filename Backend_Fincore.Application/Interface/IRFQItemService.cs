using Backend_Fincore.Application.DTOs.RFQItem;
using Backend_Fincore.Response;

namespace Backend_Fincore.Application.Interfaces
{
    public interface IRFQItemService
    {
        Task<ApiResponse<RFQItemResponseDto>> CreateAsync(RFQItemCreateDto dto);
        Task<ApiResponse<List<RFQItemResponseDto>>> GetByRfqIdAsync(int rfqId);
        Task<ApiResponse<RFQItemResponseDto>> UpdateAsync(int id, RFQItemUpdateDto dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}