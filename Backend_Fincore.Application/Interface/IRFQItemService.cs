using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.RFQItem;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public interface IRFQItemService
    {
        Task<List<RFQItemResponseDto>> GetByRfqIdAsync(int rfqId, PaginationDTO pagination);
        Task<int> GetCountByRfqIdAsync(int rfqId);
        Task CreateAsync(RFQItemCreateDto dto);
        Task UpdateAsync(int id, RFQItemUpdateDto dto);
        Task DeleteAsync(int id);
        Task<List<RFQItemResponseDto>> ReadbyRFQId(RFQItemReadbyRfqDTO data);
    }
}