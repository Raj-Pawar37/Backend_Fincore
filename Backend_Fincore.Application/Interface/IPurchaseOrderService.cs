using Backend_Fincore.DTOs.PurchaseOrder;

namespace Backend_Fincore.Interface
{
    public interface IPurchaseOrderService
    {
        Task<List<PurchaseOrderDTO>> GetAllPurchasedOrder();

        Task<PurchaseOrderDTO> GetPurchaseOrderById(int purchasedId);

        Task<bool> DeletePurchaseOrderById(int purchasedId);

        Task AddPurchaseOrderData(PurchaseOrderCUDTO PO);

        Task UpdatePurchaseOrder(PurchaseOrderCUDTO Po, int id);
    }
}
