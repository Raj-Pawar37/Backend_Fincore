using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Backend_Fincore.DTOs;

namespace Backend_Fincore.Interface
{
    public interface IBudgetLineService
    {
        Task<BudgetLineReadDTO> AddBudgetLine(BudgetLineWriteDTO dto);

        Task<List<BudgetLineReadDTO>> GetAll();

        Task<BudgetLineReadDTO?> GetById(int id);

        Task<bool> UpdateBudgetLine(int id, BudgetLineWriteDTO dto);

        Task<bool> DeleteBudgetLine(int id);
    }
}
