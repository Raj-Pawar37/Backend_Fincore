using Backend_Fincore.Application.DTOs.RFQItem;
using Backend_Fincore.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interfaces
{
    public interface IRFQItemService
    {
        Task<ApiResponse<RFQItemResponseDto>> CreateAsync(RFQItemCreateDto dto, int userId);
        Task<ApiResponse<List<RFQItemResponseDto>>> GetByRfqIdAsync(int rfqId, int pageNumber, int pageSize);
        Task<ApiResponse<RFQItemResponseDto>> UpdateAsync(int id, RFQItemUpdateDto dto, int userId);

        // Updated to accept userId for the Soft Delete audit trail
        Task<ApiResponse<bool>> DeleteAsync(int id, int userId);
    }
}