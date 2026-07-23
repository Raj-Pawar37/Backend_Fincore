using Backend_Fincore.DTOs;

namespace Backend_Fincore.Interface
{
    public interface ICapexRequestService
    {
        Task<List<BudgetLineDropdownDTO>> GetBudgetLineDropdown(string? searchText,int? departmentId);

        Task<CapexReadDTO> AddCapexRequest(CapexWriteDTO dto);

        Task<List<CapexReadDTO>> GetAll(
            int userId,
            int pageNumber,
            int pageSize);

        Task<CapexReadDTO?> GetById(int capexRequestId);

        Task<bool> UpdateCapexRequest(int capexRequestId,int userId,CapexWriteDTO dto); 

        Task<bool> DeleteCapexRequest(int capexRequestId,int userId);

        Task<bool> VerifyCapexRequest(CapexVerifyDTO dto);
    }
}