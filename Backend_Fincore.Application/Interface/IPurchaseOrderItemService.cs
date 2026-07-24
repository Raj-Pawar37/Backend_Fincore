using Backend_Fincore.Application.DTOs.PurchaseOrderItem;
using Backend_Fincore.DTOs.PurchaseOrderItem;

namespace Backend_Fincore.Interface
{
    public interface IPurchaseOrderItemService
    {
        Task<PurchaseOrderItemDTO> getAllItem(ReadPoItemsDTO poItem);

        Task<PurchaseOrderItemDTO> getItemById(int id);

        Task AddPurchasedItem(PurchaseOrderItemCUDTO PT);

        Task UpdatePurchaseOrderItem(PurchaseOrderItemCUDTO dto, int id);

        Task<bool> DeleteItem(int id);
    }
}
