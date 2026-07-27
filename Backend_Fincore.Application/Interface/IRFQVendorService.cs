using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.RFQVendor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interfaces
{
    public interface IRFQVendorService
    {
        Task<List<RFQVendorResponseDto>> GetByRfqIdAsync(int rfqId, PaginationDTO pagination);
        Task<int> GetCountByRfqIdAsync(int rfqId);
        Task CreateAsync(RFQVendorCreateDto dto);
        Task UpdateAsync(int id, RFQVendorUpdateDto dto);
        Task DeleteAsync(int id);
    }
}