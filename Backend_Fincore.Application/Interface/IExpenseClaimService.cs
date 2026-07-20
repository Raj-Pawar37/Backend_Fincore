using Backend_Fincore.DTOs;


namespace Backend_Fincore.Interface
{
    public interface IExpenseClaimService
    {
        Task<List<ExpenseClaimDto>> GetAllExpenseClaims();
        Task Create(ExpenseClaimDto opd);

        Task<ExpenseClaimDto> GetById(int id);

        Task Update(ExpenseClaimDto dto);

        Task Delete(int id);
    }
}
