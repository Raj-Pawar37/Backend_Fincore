using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.DTOs;

namespace Backend_Fincore.Application.Interface
{
    public interface IBudgetLineService
    {
        Task<List<BudgetLineDropdownDTO>> GetBudgetLineDropdown(string? searchText, int? departmentId,string? costCenter);
        Task<BudgetLineReadDTO> AddBudgetLine(BudgetLineWriteDTO dto);

        Task<List<BudgetLineReadDTO>> GetAll(PaginationDTO pagination);
        Task<int> GetTotalRecord();

        Task<BudgetLineReadDTO?> GetById(int id);

        Task<bool> UpdateBudgetLine(int id, BudgetLineWriteDTO dto);

        Task<bool> DeleteBudgetLine(int id);
    }
}
