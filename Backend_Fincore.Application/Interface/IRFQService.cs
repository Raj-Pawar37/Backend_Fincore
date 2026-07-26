using Backend_Fincore.Application.DTOs.RFQ;
using Backend_Fincore.Response;

namespace Backend_Fincore.Application.Interface
{
    public interface IRFQService
    {
        Task<ApiResponse<RFQResponseDto>> CreateAsync(RFQCreateDto dto, int userId);
        Task<ApiResponse<List<RFQResponseDto>>> GetAllAsync(int userId, int pageNumber, int pageSize);
        Task<ApiResponse<RFQResponseDto>> GetByIdAsync(int id);
        Task<ApiResponse<RFQResponseDto>> UpdateAsync(int id, RFQUpdateDto dto, int userId);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}