using Backend_Fincore.Application.DTOs;
using Backend_Fincore.DTOs;

namespace Backend_Fincore.Application.Interface
{
    public interface ICapexRequestService
    {
       // Task<List<BudgetLineDropdownDTO>> GetBudgetLineDropdown(string? searchText,int? departmentId);

        Task<CapexReadDTO> AddCapexRequest(CapexWriteDTO dto);

        Task<List<CapexReadDTO>> GetAll(PaginationDTO pagination);
        Task<int> GetTotalRecord();

        Task<CapexReadDTO?> GetById(int capexRequestId);

        Task<bool> UpdateCapexRequest(int capexRequestId,CapexWriteDTO dto); 

        Task<bool> DeleteCapexRequest(int capexRequestId);

        Task<bool> VerifyCapexRequest(int capexRequestId ,CapexVerifyDTO dto);
        Task<List<CapexVerifyDropdownDTO>> GetCapexVerifyDropdown(string? searchText);
    }
}