using Backend_Fincore.Application.DTOs.ExpenseClaim;
using Backend_Fincore.DTOs;


namespace Backend_Fincore.Interface
{
        public interface IExpenseClaimService
        {
            Task<List<ExpenseClaimReadDTO>> GetAll();

            Task<ExpenseClaimReadDTO?> GetById(int id);

            Task<ExpenseClaimReadDTO> Create(
                ExpenseClaimWriteDTO dto);

            Task<ExpenseClaimReadDTO?> Update(
                int id,
                ExpenseClaimWriteDTO dto);

            Task<bool> Delete(int id);
        }
  
}
