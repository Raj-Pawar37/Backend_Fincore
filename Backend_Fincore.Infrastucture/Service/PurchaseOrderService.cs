using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.AccountMaster;
using Backend_Fincore.Application.DTOs.PurchaseOrder;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs.PurchaseOrder;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace Backend_Fincore.Infrastucture.Service
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly AppDbContext db;

        IMapper mapper;

        private readonly ICurrentUserService current;

        private readonly IMemoryCache cache;
        public PurchaseOrderService(AppDbContext db, IMapper mapper, ICurrentUserService current, IMemoryCache cache)
        {
            this.db = db;

            this.mapper = mapper;
            this.current = current;
            this.cache = cache;
        }

        public async Task AddPurchaseOrderData(PurchaseOrderCUDTO PO)
        {

            var quotation = await db.Quotation.Include(x => x.RFQVendor)
                                 .FirstOrDefaultAsync(x => x.QuotationId == PO.QuotationId && x.IsActive == 1);


            if (quotation == null)
            {
                throw new Exception("quotation not found");
            }

            var vendor = await db.RFQVendor.FirstOrDefaultAsync(x => x.VendorId == PO.VendorId && x.IsActive == 1);

            if (vendor == null)
            {
                throw new Exception("vendor not found");
            }

            if (quotation.RFQVendor.VendorId != PO.VendorId)
            {
                throw new Exception("Selected vendor does not belong to the selected quotation.");
            }

            var poExist = await db.PurchaseOrder.FirstOrDefaultAsync(x => x.QuotationId == PO.QuotationId && x.IsActive == 1);

            if (poExist != null)
            {
                throw new Exception("Purchsed order for this quotation is alaredy exists");
            }


            var poName = await db.PurchaseOrder.FirstOrDefaultAsync(x => x.PONumber == PO.PONumber && x.IsActive == 1);

            if (poName != null)
            {
                throw new Exception("Purchased order number or name alrady exists");
            }


            var quotationItems = await db.QuotationItem.Include(x => x.RFQItem)
                                .Where(x => x.QuotationId == PO.QuotationId && x.IsActive == 1 && x.Status == "Selected")
                                .ToListAsync();

            if (!quotationItems.Any())
            {
                throw new Exception("quotation item not found");
            }


            var purchaseOrder = mapper.Map<PurchaseOrder>(PO);


            purchaseOrder.Status = "Draft";
            purchaseOrder.TotalAmount = 0;
            purchaseOrder.CreatedBy = current.UserId;


            await db.PurchaseOrder.AddAsync(purchaseOrder);
            await db.SaveChangesAsync();


            decimal totalAmount = 0;

            foreach (var items in quotationItems)
            {
                var PoItems = mapper.Map<PurchaseOrderItem>(items);



                PoItems.PurchaseOrderId = purchaseOrder.PurchaseOrderId;
                PoItems.CreatedBy = current.UserId;
                PoItems.Status = "Pending";

                decimal subTotal = PoItems.Qty * PoItems.UnitPrice;
              

                totalAmount += subTotal + items.Tax - items.Discount;

                await db.PurchaseOrderItem.AddAsync(PoItems);

                  

            }

            purchaseOrder.TotalAmount = totalAmount;

            await db.SaveChangesAsync();

            //cache.Remove($"PO_{purchaseOrder.PurchaseOrderId}");


        }


        public async Task DeletePurchaseOrderById(int purchasedId)
        {
            var purchaseOrder = await db.PurchaseOrder.FirstOrDefaultAsync(x => x.PurchaseOrderId == purchasedId && x.IsActive == 1);

            if (purchaseOrder == null)
            {
                throw new Exception("Purchased order not exists");
            }

            if (purchaseOrder.Status == "Issued" || purchaseOrder.Status == "Completed")
            {
                throw new Exception("Issued or Completed Purchase Orders cannot be deleted.");
            }

            var poItemsList = await db.PurchaseOrderItem.Where(x => x.PurchaseOrderId == purchasedId && x.IsActive == 1).ToListAsync();



            foreach (var item in poItemsList)
            {
                item.IsActive = 0;
                item.ModifiedBy = current.UserId;
                item.ModifiedAt = DateTime.Now;
            }


            purchaseOrder.IsActive = 0;
            purchaseOrder.ModifiedBy = current.UserId;
            purchaseOrder.ModifiedAt = DateTime.Now;
            await db.SaveChangesAsync();


            cache.Remove($"PO_{purchasedId}");



        }

        public async Task<int> GetPurchasedOrderCount()
        {
            return await db.PurchaseOrder.CountAsync(x=> x.IsActive == 1);
        }

        public async Task<List<PurchaseOrderDTO>> GetAllPurchasedOrder(PurchasedOrderFilterDTO pof, PaginationDTO pagination)
        {
      
            var user = await db.User.Include(x => x.Role).FirstOrDefaultAsync(x => x.UserId == current.UserId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

                if (user.Role == null)
                {
                    throw new Exception("Role not Exist");
                }

                IQueryable<PurchaseOrder> query = db.PurchaseOrder.Include(x => x.Vendor)
                                                  .Include(x => x.Quotation).Where(x => x.IsActive == 1);

                if (user.Role.RoleName == "User" || user.Role.RoleName == "Employee")
                {
                    throw new Exception("You are not authorized.");
                }

                //manager / senior manager/hod  filter

                else if (user.Role.RoleName == "Manager" || user.Role.RoleName == "Senior Manager")

                {
                    var employee = await db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == user.MasterId && x.IsActive == 1);

                    if (employee == null)
                    {
                        throw new Exception("Employee not found");
                    }
                var empIds = await db.Employee.Where(x => x.DepartmentId == employee.DepartmentId && x.IsActive == 1)
                                                  .Select(x => x.EmployeeId).ToListAsync();

                var userIds = await db.User.Where(x => x.MasterType == "Employee" && empIds
                                    .Contains(x.MasterId) && (x.Role.RoleName == "Manager"
                                    ||  x.Role.RoleName == "Senior Manager"))
                                    .Select(x => x.UserId).ToListAsync();


                query = query.Where(x => userIds.Contains(x.CreatedBy));
                }

                // Vendor
                else if (user.Role.RoleName == "Vendor")
                {
                    var vendor = await db.RFQVendor.FirstOrDefaultAsync(x => x.VendorId == user.MasterId && x.IsActive == 1);


                    if (vendor == null)
                    {
                        throw new Exception("Vendor not found.");
                    }

                    query = query.Where(x => x.VendorId == user.MasterId);
                }

                else if (user.Role.RoleName == "CFO")
                {

                }
                else
                {
                    throw new Exception("Invalid Role.");
                }

                if (!string.IsNullOrWhiteSpace(pof.Status))
                {
                    query = query.Where(x => x.Status == pof.Status);
                }


                if (!string.IsNullOrWhiteSpace(pagination.Search))
                {
                    query = query.Where(x =>
                        x.PONumber.Contains(pagination.Search) ||
                        x.Status.Contains(pagination.Search));
                }

                var purchaseOrders = await query.OrderByDescending(x => x.CreatedAt).Skip((pagination.PageNumber - 1) * pagination.PageSize)
                                           .Take(pagination.PageSize)
                                           .ToListAsync();

                var result = mapper.Map<List<PurchaseOrderDTO>>(purchaseOrders);

            //    var options = new MemoryCacheEntryOptions()
            //                                               .SetSlidingExpiration(TimeSpan.FromMinutes(5))
            //                                               .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));

            //    cache.Set(cacheKey, result, options);
            //}


            return result;


        }

        public async Task<PurchaseOrderDTO> GetPurchaseOrderById(int purchasedId)
        {
            string cacheKey = $"PO_{purchasedId}";


            if (!cache.TryGetValue(cacheKey, out PurchaseOrderDTO data))
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

                var res = await db.PurchaseOrder.FirstOrDefaultAsync(x => x.PurchaseOrderId == purchasedId && x.IsActive == 1);

                if (res == null)
                {
                    throw new Exception("Purchased order not exists");
                }


            var data = mapper.Map<PurchaseOrderDTO>(res);

            return data;

        }


        }

        public async Task UpdatePurchaseOrder(PurchaseOrderCUDTO Po, int id)
        {
            var purchasedOrder = await db.PurchaseOrder.FirstOrDefaultAsync(x => x.PurchaseOrderId == id && x.IsActive == 1);

            if (purchasedOrder == null)
            {
                throw new Exception("Purchased order not exists");
            }

            bool quotationExist = await db.PurchaseOrder.AnyAsync(x => x.QuotationId == Po.QuotationId && x.PurchaseOrderId != id && x.IsActive == 1);


            if (quotationExist)
            {
                throw new Exception("Purchase Order already exists for this quotation.");
            }

            bool exists = await db.PurchaseOrder.AnyAsync(x => x.PONumber == Po.PONumber && x.PurchaseOrderId != id && x.IsActive == 1);


            if (exists)
            {
                throw new Exception("Purchase Order Number already exists.");
            }

            if (purchasedOrder.Status == "Issued" || purchasedOrder.Status == "Completed")
            {
                throw new Exception("Issued or Completed Purchase Orders cannot be updated.");
            }

            purchasedOrder.VendorId = Po.VendorId;
            purchasedOrder.QuotationId = Po.QuotationId;
            purchasedOrder.PONumber = Po.PONumber;
            purchasedOrder.ModifiedAt = DateTime.Now;
            purchasedOrder.ModifiedBy = current.UserId;


            await db.SaveChangesAsync();

            cache.Remove($"PO_{id}");

        }


        public async Task UpdatePOStatus(int purchaseOrderId, PurchasedOrderStatusDTO dto)
        {
            var purchasedOrder = await db.PurchaseOrder.FirstOrDefaultAsync(x => x.PurchaseOrderId == purchaseOrderId && x.IsActive == 1);

            if (purchasedOrder == null)
            {

                throw new Exception("Purchase Order not found.");
            }

            if (purchasedOrder.Status == "Completed")
            {
                throw new Exception("Completed Purchase Order cannot be updated.");
            }

            if (purchasedOrder.Status == "Cancelled")
            {
                throw new Exception("Cancelled Purchase Order cannot be updated.");
            }


            if (purchasedOrder.Status != "Draft")
            {

                throw new Exception("Only Draft Purchase Orders can be Updated.");
            }

            if (dto.Status != "Issued" && dto.Status != "Cancelled")
            {
                throw new Exception("Purchase Order status can only be changed to Issued or Cancelled.");
            }

            if (dto.Status == "Issued")
            {
                bool hasItems = await db.PurchaseOrderItem
                    .AnyAsync(x => x.PurchaseOrderId == purchaseOrderId && x.IsActive == 1);

                if (!hasItems)
                {
                    throw new Exception("Purchase Order must contain at least one item.");
                }

                if (purchasedOrder.TotalAmount <= 0)
                {
                    throw new Exception("Purchase Order total amount should be greater than zero.");
                }
            }

            purchasedOrder.Status = dto.Status;
            purchasedOrder.ModifiedBy = current.UserId;
            purchasedOrder.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            cache.Remove($"PO_{purchaseOrderId}");

        }

       

       
    }
}
