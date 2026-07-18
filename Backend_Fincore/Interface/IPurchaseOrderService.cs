using Backend_Fincore.DTOs.PurchaseOrder;

namespace Backend_Fincore.Interface
{
    public interface IPurchaseOrderService
    {
        Task<List<PurchaseOrderDTO>> GetAllPurchasedOrder();

        Task<PurchaseOrderDTO> GetPurchaseOrderById(int purchasedId);
    }
}
