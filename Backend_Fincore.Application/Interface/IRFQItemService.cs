using Backend_Fincore.Application.DTOs.RFQItem;
using Backend_Fincore.Response;

namespace Backend_Fincore.Application.Interface
{
    public interface IRFQItemService
    {
        Task<ApiResponse<RFQItemResponseDto>> CreateAsync(RFQItemCreateDto dto, int userId);
        Task<ApiResponse<List<RFQItemResponseDto>>> GetByRfqIdAsync(int rfqId, int pageNumber, int pageSize);
        Task<ApiResponse<RFQItemResponseDto>> UpdateAsync(int id, RFQItemUpdateDto dto, int userId);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}