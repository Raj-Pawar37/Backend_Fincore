using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.PurchaseOrderItem;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs.PurchaseOrderItem;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Reflection;

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
                
                purchaseOrder.ModifiedAt = DateTime.Now;

                await db.SaveChangesAsync();
            }
        }

        public async Task<int> GetPurchasedItemCount()
        {
            return await db.PurchaseOrderItem.CountAsync();
        }

        public async Task<List<PurchaseOrderItemDTO>> getAllPurchasedItem(ReadPoItemsDTO poItem,PaginationDTO pagination)
        {
            var search =  db.PurchaseOrderItem.AsQueryable();

            if (!string.IsNullOrEmpty(pagination.Search))
            {
                search = search.Where(x =>
                    x.ItemName.Contains(pagination.Search) ||

                    x.ItemType.Contains(pagination.Search) ||

                    x.Status.Contains(pagination.Search)

                    );
            }

            var data = await search
                                   .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                                   .Take(pagination.PageSize)
                                   .ToListAsync();

            mapper.Map<PurchaseOrderItemDTO>(data);



            var user = await db.User.Include(x=>x.Role).FirstOrDefaultAsync(x => x.UserId == poItem.userId);

            if(user == null)
            {
                throw new Exception("user not found");
            }

            if(user.Role == null)
            {
                throw new Exception("Role not exists");
            }

            var poi = await db.PurchaseOrderItem.Include(x=>x.PurchaseOrder)
                         .FirstOrDefaultAsync(x => x.POItemId == poItem.poItemId);

            if(poi == null)
            {
                throw new Exception("Purchased order item not found");
            }

            if(user.Role.RoleName == "User")
            {
                throw new Exception("You are not authorized.");
            }

            //Manager 
            else if(user.Role.RoleName == "Manager")
            {
                var managerEmp = await db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == user.MasterId);

                if(managerEmp == null)
                {
                    throw new Exception("Employee not found.");

                }

                var createdUser = await db.User.FirstOrDefaultAsync(x => x.UserId == poi.CreatedBy);


                if (createdUser == null)
                {
                    throw new Exception("Purchase Order creator not found.");
                }

                var creatorEmployee = await db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == createdUser.MasterId);


                if (creatorEmployee == null)
                {
                    throw new Exception("Employee not found.");
                }

                if (managerEmp.DepartmentId != creatorEmployee.DepartmentId)
                {
                    throw new Exception("You are not authorized.");
                }

            }

            //vendor

            else if (user.Role.RoleName == "Vendor")
            {
                if (poi.PurchaseOrder.VendorId != user.MasterId)
                    throw new Exception("You are not authorized.");
            }

            // CFO
            else if (user.Role.RoleName == "CFO")
            {
                // No restriction
            }
            else
            {
                throw new Exception("Invalid role.");
            }

            return mapper.Map<PurchaseOrderItemDTO>(poi);
 
        }

        public async Task<PurchaseOrderItemDTO> getItemById(int id)
        {
            var item = await db.PurchaseOrderItem.Include(x => x.PurchaseOrder)
                            .FirstOrDefaultAsync(x => x.PurchaseOrderId == id);

            if (item == null)
            {
                throw new Exception("Purchase Order Item not found.");
            }

            return mapper.Map<PurchaseOrderItemDTO>(item);
        }

        public async Task AddPurchasedItem(PurchaseOrderItemCUDTO POI)
        {
            var purchaseOrder = await db.PurchaseOrder.FirstOrDefaultAsync(x => x.PurchaseOrderId == POI.PurchaseOrderId);


            if (purchaseOrder == null)
            {
                throw new Exception("Purchase Order not found.");
            }


            var purchaseOrderItemExist = await db.PurchaseOrderItem.FirstOrDefaultAsync(x => x.QuotationItemId == POI.QuotationItemId);

            if(purchaseOrderItemExist != null)
            {
                throw new Exception("Purchase order item for this quotation item already exist");
            }

            
            var item = mapper.Map<PurchaseOrderItem>(POI);

            item.CreatedBy = POI.CreatedBy;

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

            bool itemExists = await db.PurchaseOrderItem.AnyAsync(x => x.PurchaseOrderId == item.PurchaseOrderId &&
                                                                       x.ItemName == POI.ItemName &&
                                                                       x.POItemId != id);

            if (itemExists)
            {
                throw new Exception("Purchase Order Item already exists.");
            }

            mapper.Map(POI, item);


            item.ModifiedAt = DateTime.Now;
            item.ModifiedBy = POI.ModifiedBy;
            
            await db.SaveChangesAsync();

            // Update Purchase Order Total
            await UpdatePurchaseOrderTotal(item.PurchaseOrderId);
        }

        public async Task<bool> DeleteItem(int id)
        {
            var data = await db.PurchaseOrderItem.FirstOrDefaultAsync(x => x.POItemId == id);


            if (data == null)
            {
                throw new Exception("Purchase Order Item not found.");
            }

            int purchaseOrderId = data.PurchaseOrderId;

            db.PurchaseOrderItem.Remove(data);

            await db.SaveChangesAsync();

            await UpdatePurchaseOrderTotal(purchaseOrderId);

            return true;
                

        }
    }
}
