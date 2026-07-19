using Backend_Fincore.DTOs;
using Backend_Fincore.Models;

namespace Backend_Fincore.Interface
{
    public interface IExpenseClaimService
    {
        Task<List<ExpenseClaim>> GetAllExpenseClaims();
        Task Create(ExpenseClaimDto opd);

        Task<ExpenseClaim> GetById(int id);

        Task Update(ExpenseClaimDto dto);

        Task Delete(int id);
    }
}
