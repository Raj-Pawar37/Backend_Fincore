using Backend_Fincore.Application.DTOs;
using Backend_Fincore.DTOs;

namespace Backend_Fincore.Application.Interface
{
    public interface IBudgetCategoryService
    {
        Task<List<BudgetCategoryDropdownDTO>>GetBudgetCategoryDropdown(string? search);
        Task<BudgetCategoryReadDTO> AddBudgetCategory(BudgetCategoryWriteDTO dto);

        Task<List<BudgetCategoryReadDTO>> GetAll(PaginationDTO pagination);
        Task <int>  GetTotalRecord();

        Task<BudgetCategoryReadDTO?> GetById(int id);

        Task<bool> UpdateBudgetCategory(int id,BudgetCategoryWriteDTO dto);

        Task<bool> DeleteBudgetCategory(int id);
    }
}