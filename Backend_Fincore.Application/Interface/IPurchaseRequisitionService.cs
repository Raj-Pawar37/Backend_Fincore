using Backend_Fincore.Application.DTOs.PurchaseRequisition;
using Backend_Fincore.Response;

namespace Backend_Fincore.Application.Interfaces
{
    public interface IPurchaseRequisitionService
    {
        Task<ApiResponse<List<PurchaseRequisitionResponseDto>>> GetAllAsync();
        Task<ApiResponse<PurchaseRequisitionResponseDto>> GetByIdAsync(int id);
        Task<ApiResponse<PurchaseRequisitionResponseDto>> CreateAsync(PurchaseRequisitionCreateDto dto);
        Task<ApiResponse<PurchaseRequisitionResponseDto>> UpdateAsync(int id, PurchaseRequisitionUpdateDto dto);

        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}