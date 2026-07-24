using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.PurchaseOrder;
using Backend_Fincore.Application.DTOs.PurchaseOrderItem;
using Backend_Fincore.DTOs.PurchaseOrder;
using Backend_Fincore.Models;

namespace Backend_Fincore.Interface
{
    public interface IPurchaseOrderService
    {
        Task<List<PurchaseOrderDTO>> GetAllPurchasedOrder(PurchasedOrderFilterDTO pof);

        Task<PurchaseOrderDTO> GetPurchaseOrderById(int purchasedId);

        Task DeletePurchaseOrderById(int purchasedId);

        Task AddPurchaseOrderData(PurchaseOrderCUDTO PO);

        Task UpdatePurchaseOrder(PurchaseOrderCUDTO Po, int id);

        Task UpdatePOStatus(int purchaseOrderId, PurchasedOrderFilterDTO dto);

      



    }
}
