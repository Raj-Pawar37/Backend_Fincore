using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.PurchaseOrderItem;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs.PurchaseOrderItem;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Backend_Fincore.Infrastucture.Service
{
    public class PurchaseOrderItemService : IPurchaseOrderItemService
    {
        private readonly AppDbContext db;

        IMapper mapper;

        private readonly ICurrentUserService current;

        public PurchaseOrderItemService(AppDbContext db,IMapper mapper,ICurrentUserService current)
        {
            this.db = db;
            this.mapper = mapper;
            this.current = current;
        }

        private async Task UpdatePurchaseOrderTotal(int purchaseOrderId)
        {
            var total = await db.PurchaseOrderItem.Where(x => x.PurchaseOrderId == purchaseOrderId && x.IsActive == 1)
                              .SumAsync(x => (x.UnitPrice * x.Qty) + (x.Tax ?? 0) - (x.Discount ?? 0));


            var purchaseOrder = await db.PurchaseOrder.FirstOrDefaultAsync(x => x.PurchaseOrderId == purchaseOrderId && x.IsActive == 1);

            if (purchaseOrder == null)
            {
                throw new Exception("Purchase Order not found.");
            }

            purchaseOrder.TotalAmount = total;
            purchaseOrder.ModifiedBy = current.UserId;
            purchaseOrder.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();
            
        }

        public async Task<int> GetPurchasedItemCount()
        {
            return await db.PurchaseOrderItem.CountAsync( x => x.IsActive == 1);
        }
        public async Task<List<PurchaseOrderItemDTO>> getAllPurchasedItem(PaginationDTO pagination)
        {
            var user = await db.User.Include(x => x.Role).FirstOrDefaultAsync(x => x.UserId == current.UserId && x.IsActive == 1);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            if (user.Role == null)
            {
                throw new Exception("Role not found.");
            }

            IQueryable<PurchaseOrderItem> query = db.PurchaseOrderItem.Include(x => x.PurchaseOrder).Where(x => x.IsActive == 1);


            switch (user.Role.RoleName)
            {
                case "Administrator":
                case "Procurement Manager":
                case "Warehouse Manager":
                case "Asset Manager":
                case "CFO":
                   
                    break;

                case "Vendor":
                                   query = query.Where(x => x.PurchaseOrder.VendorId == user.MasterId);
                                   break;

                case "User":
                                   throw new Exception("You are not authorized.");

                default:
                                    throw new Exception("Invalid role.");
            }

            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                query = query.Where(x => x.ItemName.Contains(pagination.Search) ||
                                         x.Status.Contains(pagination.Search) ||
                                         x.PurchaseOrder.PONumber.Contains(pagination.Search));

            }

            var result = await query.OrderByDescending(x => x.CreatedAt)
                                    .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                                    .Take(pagination.PageSize)
                                    .ToListAsync();


            return mapper.Map<List<PurchaseOrderItemDTO>>(result);
        }

        public async Task<PurchaseOrderItemDTO> getItemById(int id)
        {
            var user = await db.User.Include(x => x.Role).FirstOrDefaultAsync(x => x.UserId == current.UserId && x.IsActive == 1);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            if (user.Role == null)
            {
                throw new Exception("Role not found.");
            }

            var item = await db.PurchaseOrderItem.Include(x => x.PurchaseOrder).FirstOrDefaultAsync(x => x.POItemId == id && x.IsActive == 1);

            if (item == null)
            {
                throw new Exception("Purchase Order Item not found.");
            }

            switch (user.Role.RoleName)
            {
                case "Administrator":
                case "Procurement Manager":
                case "Warehouse Manager":
                case "Asset Manager":
                case "CFO":
                    
                    break;

                case "Vendor":

                    if (item.PurchaseOrder.VendorId != user.MasterId)
                    {
                        throw new Exception("You are not authorized to view this Purchase Order Item.");
                    }

                    break;

                case "User":
                    throw new Exception("You are not authorized.");

                default:
                    throw new Exception("Invalid role.");
            }

            return mapper.Map<PurchaseOrderItemDTO>(item);
        }

     

        public async Task UpdatePurchaseOrderItem(PurchaseOrderItemCUDTO POI, int id)
        {

            var item = await db.PurchaseOrderItem.Include(x => x.PurchaseOrder).FirstOrDefaultAsync(x => x.POItemId == id && x.IsActive == 1);

            if (item == null)
            {
                throw new Exception("Purchase Order Item not found.");
            }

            if (item.PurchaseOrder == null || item.PurchaseOrder.IsActive != 1)
            {
                throw new Exception("Purchase Order not found.");
            }

            
            if (item.PurchaseOrder.Status != "Draft")
            {
                throw new Exception("Only Draft Purchase Orders can be edited.");
            }

            if (item.Status != "Pending")
            {
                throw new Exception("Only Pending Purchase Order Items can be edited.");
            }


            if (item.Status == "Received" )
            {
                throw new Exception("Received Purchase Order Item cannot be edited.");
            }


          
            bool itemExists = await db.PurchaseOrderItem.AnyAsync(x => x.PurchaseOrderId == item.PurchaseOrderId &&
                                                                       x.ItemName == POI.ItemName &&
                                                                       x.POItemId != id &&
                                                                       x.IsActive == 1);


            if (itemExists)
            {
                throw new Exception("Purchase Order Item already exists.");
            }

            
            item.ItemName = POI.ItemName;
            item.UnitPrice = POI.UnitPrice;
            item.Qty = POI.Qty;
            item.Tax = POI.Tax;
            item.Discount = POI.Discount;

            item.ModifiedBy = current.UserId;
            item.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

           
            await UpdatePurchaseOrderTotal(item.PurchaseOrderId);
        }

        public async Task DeleteItem(int id)
        {
            var item = await db.PurchaseOrderItem.Include(x => x.PurchaseOrder).FirstOrDefaultAsync(x => x.POItemId == id && x.IsActive == 1);

            if (item == null)
            {
                throw new Exception("Purchase Order Item not found.");
            }

            if (item.PurchaseOrder == null || item.PurchaseOrder.IsActive != 1)
            {
                throw new Exception("Purchase Order not found.");
            }

          
            if (item.PurchaseOrder.Status != "Draft")
            {
                throw new Exception("Only Draft Purchase Orders can be modified.");
            }

            if (item.Status != "Pending")
            {
                throw new Exception("Only Pending Purchase Order Items can be deleted.");
            }

            if (item.Status == "Received")
            {
                throw new Exception("Received Purchase Order Item cannot be deleted.");
            }

            item.IsActive = 0;
            item.ModifiedBy = current.UserId;
            item.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            
            await UpdatePurchaseOrderTotal(item.PurchaseOrderId);
        }

        public async Task<List<POItemsSearchDTO>> SearchPOItem(SearchPoiDTO dto)
        {

            var purchaseOrder = await db.PurchaseOrder.FirstOrDefaultAsync(x => x.PurchaseOrderId == dto.PurchaseOrderId && x.IsActive == 1);


            if (purchaseOrder == null)
            {
                throw new Exception("Purchase Order not found.");
            }

            if (purchaseOrder.Status != "Issued")
            {
                throw new Exception("GRN can only be created for Issued Purchase Orders.");
            }

            IQueryable<PurchaseOrderItem> query = db.PurchaseOrderItem.Where(x =>
                                                    x.PurchaseOrderId == dto.PurchaseOrderId &&
                                                    x.IsActive == 1 &&
                                                    (x.Status == "Pending" || x.Status == "Partially Received"));


           
            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                query = query.Where(x => x.Status == dto.Status);
            }

           
            if (!string.IsNullOrWhiteSpace(dto.SearchText))
            {
                query = query.Where(x => x.ItemName.Contains(dto.SearchText));
            }

            var result = await query.OrderBy(x => x.ItemName).Take(20).Select(x => new POItemsSearchDTO
                                                                              {
                                                                                  POItemId = x.POItemId,
                                                                                  ItemName = x.ItemName,
                                                                                  Qty = x.Qty,
                                                                                  UnitPrice = x.UnitPrice,
                                                                                  Status = x.Status
                                                                              }).ToListAsync();

            return result;

        }
    }
}
