using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.PurchaseOrderItem;
using Backend_Fincore.DTOs.PurchaseOrder;
using Backend_Fincore.Models;

namespace Backend_Fincore.Interface
{
    public interface IPurchaseOrderService
    {
        Task<List<PurchaseOrderDTO>> GetAllPurchasedOrder();

        Task<PurchaseOrderDTO> GetPurchaseOrderById(int purchasedId);

        Task<bool> DeletePurchaseOrderById(int purchasedId);

        Task AddPurchaseOrderData(PurchaseOrderCUDTO PO);

        Task UpdatePurchaseOrder(PurchaseOrderCUDTO Po, int id);

        // Business work flow api
        Task CreatePOFromQuotation(SelectedQuotationDTO selectedQuotation);

        Task<List<PurchaseOrderDTO>> GetDepartmentPurchaseOrders(int userId);

        Task UpdatePOStatus(int purchaseOrderId, UpdatePoStatusDTO dto);

        Task<List<PurchaseOrderDTO>> GetVendorIssuedPurchaseOrders(int vendorId);



    }
}
