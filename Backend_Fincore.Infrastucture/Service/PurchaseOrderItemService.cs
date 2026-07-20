using AutoMapper;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs.PurchaseOrderItem;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Service
{
    public class PurchaseOrderItemService : IPurchaseOrderItemService
    {
        private readonly AppDbContext db;

        IMapper mapper;

        public PurchaseOrderItemService(AppDbContext db,IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        private async Task UpdatePurchaseOrderTotal(int purchaseOrderId)
        {
            var total = await db.PurchaseOrderItem.Where(x => x.PurchaseOrderId == purchaseOrderId)
                       .SumAsync(x => (x.UnitPrice * x.Qty)
                       + ((x.UnitPrice * x.Qty) * (x.Tax ?? 0) / 100)
                       - ((x.UnitPrice * x.Qty) * (x.Discount ?? 0) / 100));


            var purchaseOrder = await db.PurchaseOrder.FirstOrDefaultAsync(x => x.PurchaseOrderId == purchaseOrderId);


            if (purchaseOrder != null)
            {
                purchaseOrder.TotalAmount = total;
                await db.SaveChangesAsync();
            }
        }

        public async Task<List<PurchaseOrderItemDTO>> getAllItem()
        {
            var res = await db.PurchaseOrderItem.ToListAsync();
            var data = mapper.Map<List<PurchaseOrderItemDTO>>(res);

            return data;
        }

        public async Task<PurchaseOrderItemDTO> getItemById(int id)
        {
            var item = await db.PurchaseOrderItem.FirstOrDefaultAsync(x => x.PurchaseOrderId == id);

            if(item != null)
            {
                var data = mapper.Map<PurchaseOrderItemDTO>(item);

                return data;
            }

            return null;
        }

        public async Task AddPurchasedItem(PurchaseOrderItemCUDTO PT)
        {
            var item = mapper.Map<PurchaseOrderItem>(PT);

            item.CreatedBy = 1;

            await db.PurchaseOrderItem.AddAsync(item);

            await db.SaveChangesAsync();

            // Update Purchase Order Total

            await UpdatePurchaseOrderTotal(item.PurchaseOrderId);
        }

        public async Task UpdatePurchaseOrderItem(PurchaseOrderItemCUDTO POI, int id)
        {
            var item = await db.PurchaseOrderItem.FirstOrDefaultAsync(x => x.POItemId == id);

            if( item == null)
            {
                throw new Exception("Purchase Order Item not found.");
            }

            item.ItemName = POI.ItemName;
            item.ItemType = POI.ItemType;
            item.UnitPrice = POI.UnitPrice;
            item.Tax = POI.Tax;
            item.Discount = POI.Discount;
            item.Qty = POI.Qty;
            item.PurchaseOrderId = POI.PurchaseOrderId;

            // Temporary until JWT authentication is implemented

            //item.ModifiedBy = userid

            item.ModifiedBy = 1;
            item.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            // Update Purchase Order Total
            await UpdatePurchaseOrderTotal(item.PurchaseOrderId);
        }

        public async Task<bool> DeleteItem(int id)
        {
            var data = await db.PurchaseOrderItem.FirstOrDefaultAsync(x => x.POItemId == id);

            if( data != null)
            {
                db.PurchaseOrderItem.Remove(data);
                db.SaveChangesAsync();

                return true;
            }

            return false;
                    

        }
    }
}
