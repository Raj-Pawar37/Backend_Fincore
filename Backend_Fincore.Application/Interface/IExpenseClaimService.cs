using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.ExpenseClaim;
using Backend_Fincore.DTOs;


namespace Backend_Fincore.Interface
{
        public interface IExpenseClaimService
        {
            Task<List<ExpenseClaimReadDTO>> GetAll(int userId, PaginationDTO Pagination);

            Task<ExpenseClaimReadDTO?> GetById(int id);

            Task<ExpenseClaimReadDTO> Create(
                ExpenseClaimWriteDTO dto);

            Task<ExpenseClaimReadDTO> Update(
             int expenseClaimId,
             ExpenseClaimWriteDTO dto);

           Task<bool> Delete(int expenseClaimId);

                Task<ExpenseClaimReadDTO> Verify(
                int expenseClaimId,
                int verifiedBy,
                ExpenseClaimVerifyDTO dto);

            Task<int> GetExpenseClaimCount(int userId,PaginationDTO pagination);
    }
  
}
