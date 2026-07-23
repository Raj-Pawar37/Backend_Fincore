using Backend_Fincore.DTOs;

namespace Backend_Fincore.Interface
{
    public interface IBudgetCategoryService
    {
        Task<BudgetCategoryReadDTO> AddBudgetCategory(BudgetCategoryWriteDTO dto);

        Task<List<BudgetCategoryReadDTO>> GetAll();

        Task<BudgetCategoryReadDTO?> GetById(int id);

        Task<bool> UpdateBudgetCategory(int id,BudgetCategoryWriteDTO dto);

        Task<bool> DeleteBudgetCategory(int id);
    }
}