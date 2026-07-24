using Backend_Fincore.Application.DTOs.PurchaseRequisition;
using Backend_Fincore.Response;

namespace Backend_Fincore.Application.Interfaces
{
    public interface IPurchaseRequisitionService
    {
        // Added parameters to handle the Role-Based Access logic
        Task<ApiResponse<List<PurchaseRequisitionResponseDto>>> GetAllAsync(int userId);
        Task<ApiResponse<PurchaseRequisitionResponseDto>> GetByIdAsync(int id);
        Task<ApiResponse<PurchaseRequisitionResponseDto>> UpdateAsync(int id, PurchaseRequisitionUpdateDto dto);
        Task<ApiResponse<List<PRDropdownResponseDto>>> GetPRDropdownAsync(string? searchText, int? departmentId);
    }
}