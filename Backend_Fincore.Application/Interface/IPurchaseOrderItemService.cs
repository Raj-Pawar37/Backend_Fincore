using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.PurchaseOrderItem;
using Backend_Fincore.DTOs.PurchaseOrderItem;

namespace Backend_Fincore.Application.Interface
{
    public interface IPurchaseOrderItemService
    {
        Task<List<PurchaseOrderItemDTO>> getAllPurchasedItem(PaginationDTO pagination);

        Task<int> GetPurchasedItemCount();

        Task<PurchaseOrderItemDTO> getItemById(int id);

        //Task AddPurchasedItem(PurchaseOrderItemCUDTO PT);

        Task UpdatePurchaseOrderItem(PurchaseOrderItemCUDTO dto, int id);

        Task DeleteItem(int id);

        //Task<List<POItemsSearchDTO>> FetchPOIBygrnId(int grnId);

        Task<List<POItemsSearchDTO>> SearchPOItem(SearchPoiDTO dto);
    }
}
