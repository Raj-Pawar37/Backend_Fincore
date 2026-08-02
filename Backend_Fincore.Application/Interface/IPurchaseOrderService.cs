using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.PurchaseOrder;

using Backend_Fincore.DTOs.PurchaseOrder;
using Backend_Fincore.Models;

namespace Backend_Fincore.Application.Interface
{
    public interface IPurchaseOrderService
    {
        Task<List<PurchaseOrderDTO>> GetAllPurchasedOrder( PaginationDTO pagination);

        Task<PurchaseOrderDTO> GetPurchaseOrderById(int purchasedId);

        Task<int> GetPurchasedOrderCount();

        Task DeletePurchaseOrderById(int purchasedId);

        Task AddPurchaseOrderData(PurchaseOrderCUDTO PO);

        Task UpdatePurchaseOrder(PurchaseOrderCUDTO Po, int id);

        Task UpdatePOStatus(int purchaseOrderId, PurchasedOrderStatusDTO dto);

        Task<List<PurchaseOrderDTO>> FetchIssuedPO();

        Task<List<PurchaseOrderDTO>> FetchIssuedPO();

      



    }
}
