using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend_Fincore.DTOs;

namespace Backend_Fincore.Interface
{
    public interface IBudgetService
    {
        Task<BudgetReadDTO> AddBudget(BudgetWriteDTO dto);

        Task<List<BudgetReadDTO>> GetAll();

        Task<BudgetReadDTO?> GetById(int id);

        Task<bool> UpdateBudget(int id, BudgetWriteDTO dto);

        Task<bool> DeleteBudget(int id);
    }
}
