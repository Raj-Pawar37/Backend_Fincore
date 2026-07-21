using Backend_Fincore.Application.DTOs;

namespace Backend_Fincore.Application.Interface
{
    public interface IRevenueEntryService
    {
        Task<List<RevenueEntryDto>> GetAllAsync();

        Task<RevenueEntryDto?> GetByIdAsync(int id);

        Task<bool> AddAsync(RevenueEntryCreateDto dto);

        Task<bool> UpdateAsync(RevenueEntryUpdateDto dto);

        Task<bool> DeleteAsync(int id);
    }
}