using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.PurchaseOrderItem;
using Backend_Fincore.DTOs.PurchaseOrderItem;

namespace Backend_Fincore.Interface
{
    public interface IPurchaseOrderItemService
    {
        Task<List<PurchaseOrderItemDTO>> getAllPurchasedItem(ReadPoItemsDTO poItem,PaginationDTO pagination);

        Task<int> GetPurchasedItemCount();

        Task<PurchaseOrderItemDTO> getItemById(int id);

        Task AddPurchasedItem(PurchaseOrderItemCUDTO PT);

        Task UpdatePurchaseOrderItem(PurchaseOrderItemCUDTO dto, int id);

        Task<bool> DeleteItem(int id);
    }
}
