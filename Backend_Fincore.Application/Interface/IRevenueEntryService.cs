using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.RevenueEntry;
using Backend_Fincore.Application.Response;
using Backend_Fincore.Response;

namespace Backend_Fincore.Application.Interface
{
    public interface IRevenueEntryService
    {
        Task<ApiResponse<List<RevenueEntryDto>>> GetAllAsync(PaginationDTO pagination);

        Task<RevenueEntryDto?> GetByIdAsync(int id);

        Task<bool> AddAsync(RevenueEntryCreateDto dto);

        Task<bool> UpdateAsync(RevenueEntryUpdateDto dto);

        Task<bool> DeleteAsync(int id);
    }
}