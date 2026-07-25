using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.RevenueEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public interface IRevenueEntryService
    {
        Task<List<RevenueEntryDto>> GetAllAsync(PaginationDTO pagination);

        Task<RevenueEntryDto?> GetByIdAsync(int id);

        Task<bool> AddAsync(RevenueEntryCreateDto dto);

        Task<bool> UpdateAsync(RevenueEntryUpdateDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
