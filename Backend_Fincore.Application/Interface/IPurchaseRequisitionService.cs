using Backend_Fincore.Application.DTOs.PurchaseRequisition;
using Backend_Fincore.Response;

namespace Backend_Fincore.Application.Interface
{
    public interface IPurchaseRequisitionService
    {
        Task<ApiResponse<List<PurchaseRequisitionResponseDto>>> GetAllAsync(int userId);

        Task<ApiResponse<PurchaseRequisitionResponseDto>> GetByIdAsync(int id);
        Task<ApiResponse<PurchaseRequisitionResponseDto>> UpdateAsync(int id, PurchaseRequisitionUpdateDto dto, int userId);
        Task<ApiResponse<List<PRDropdownResponseDto>>> GetPRDropdownAsync(string? searchText, int? departmentId);
    }
}