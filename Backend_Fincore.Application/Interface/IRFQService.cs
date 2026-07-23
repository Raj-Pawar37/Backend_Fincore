using Backend_Fincore.Application.DTOs.RFQ;
using Backend_Fincore.Response;

namespace Backend_Fincore.Application.Interfaces
{
    public interface IRFQService
    {
        Task<ApiResponse<RFQResponseDto>> CreateAsync(RFQCreateDto dto);
        Task<ApiResponse<List<RFQResponseDto>>> GetAllAsync(int userId, int pageNumber, int pageSize);
        Task<ApiResponse<RFQResponseDto>> GetByIdAsync(int id);
        Task<ApiResponse<RFQResponseDto>> UpdateAsync(int id, RFQUpdateDto dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}