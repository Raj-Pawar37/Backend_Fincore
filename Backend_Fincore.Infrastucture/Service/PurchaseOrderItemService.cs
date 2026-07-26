using AutoMapper;
using Backend_Fincore.Application.DTOs;

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
            var total = await db.PurchaseOrderItem.Where(x => x.PurchaseOrderId == purchaseOrderId)
                       .SumAsync(x => (x.UnitPrice * x.Qty)
                       + ((x.UnitPrice * x.Qty) * (x.Tax ?? 0) / 100)
                       - ((x.UnitPrice * x.Qty) * (x.Discount ?? 0) / 100));


            var purchaseOrder = await db.PurchaseOrder.FirstOrDefaultAsync(x => x.PurchaseOrderId == purchaseOrderId);


            if (purchaseOrder != null)
            {
                purchaseOrder.TotalAmount = total;

                purchaseOrder.ModifiedBy = current.UserId;
                
                purchaseOrder.ModifiedAt = DateTime.Now;

                await db.SaveChangesAsync();
            }
        }

        public async Task<int> GetPurchasedItemCount()
        {
            return await db.PurchaseOrderItem.CountAsync();
        }

        public async Task<List<PurchaseOrderItemDTO>> getAllPurchasedItem(PaginationDTO pagination)
        {

            var user = await db.User.Include(x=>x.Role).FirstOrDefaultAsync(x => x.UserId == current.UserId);

            if(user == null)
            {
                throw new Exception("user not found");
            }

            if(user.Role == null)
            {
                throw new Exception("Role not exists");
            }

            IQueryable<PurchaseOrderItem> query = db.PurchaseOrderItem.Include(x => x.PurchaseOrder);



            if(user.Role.RoleName == "User")
            {
                throw new Exception("You are not authorized.");
            }

            //Manager 
            else if(user.Role.RoleName == "Manager" || user.Role.RoleName == "HOD" || user.Role.RoleName == "Senior Manager")
            {

                var employee = await db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == user.MasterId);

                if (employee == null)
                {
                    throw new Exception("Employee not found");
                }

                var empIds = await db.Employee.Where(x => x.DepartmentId == employee.DepartmentId)
                                   .Select(x => x.EmployeeId).ToListAsync();

                var userIds = await db.User.Where(x => x.MasterType == "Employee" && empIds
                                    .Contains(x.MasterId)).Select(x => x.UserId).ToListAsync();

                query = query.Where(x => userIds.Contains(x.PurchaseOrder.CreatedBy));

            }

            else if (user.Role.RoleName == "Vendor")
            {
               
                query = query.Where(x => x.PurchaseOrder.VendorId == user.MasterId);
            }

          
            else if (user.Role.RoleName == "CFO")
            {
           
            }
            else
            {
                throw new Exception("Invalid role.");
            }

            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                query = query.Where(x =>
                    x.ItemName.Contains(pagination.Search) ||
                    x.Status.Contains(pagination.Search));
            }

            // Pagination

            var result = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return mapper.Map<List<PurchaseOrderItemDTO>>(result);

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

            item.CreatedBy = current.UserId;

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

            if (item.PurchaseOrder.Status != "Draft")
            {
                throw new Exception("Only Draft Purchase Orders can be edited.");
            }

            if (itemExists)
            {
                throw new Exception("Purchase Order Item already exists.");
            }

            if (item.Status == "Received")
            {
                throw new Exception("Received Purchase Order Item cannot be edited.");
            }

            mapper.Map(POI, item);


            item.ModifiedAt = DateTime.Now;
            item.ModifiedBy = current.UserId;
            
            await db.SaveChangesAsync();

            // Update Purchase Order Total
            await UpdatePurchaseOrderTotal(item.PurchaseOrderId);
        }

        public async Task DeleteItem(int id)
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

        }
    }
}
