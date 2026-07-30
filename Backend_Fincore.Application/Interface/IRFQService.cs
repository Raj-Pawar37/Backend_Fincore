using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.RFQ;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public interface IRFQService
    {
        Task<List<RFQResponseDto>> GetAllAsync(PaginationDTO pagination);
        Task<int> GetCountAsync();
        Task<RFQResponseDto> GetByIdAsync(int id);
        Task CreateAsync(RFQCreateDto dto);
        Task UpdateAsync(int id, RFQUpdateDto dto);
        Task DeleteAsync(int id);
        Task<List<RFQDropdownDto>> GetDropdownAsync(string? searchText, int? vendorId, string? status);
    }
}