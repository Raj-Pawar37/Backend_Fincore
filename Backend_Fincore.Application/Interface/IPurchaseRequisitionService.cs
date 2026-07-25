using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.PurchaseRequisition;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interfaces
{
    public interface IPurchaseRequisitionService
    {
        Task<List<PurchaseRequisitionResponseDto>> GetAllAsync(PaginationDTO pagination);
        Task<int> GetCountAsync();
        Task<PurchaseRequisitionResponseDto> GetByIdAsync(int id);
        Task UpdateAsync(int id, PurchaseRequisitionUpdateDto dto);
        Task<List<PRDropdownResponseDto>> GetPRDropdownAsync(string? searchText, int? departmentId);
    }
}