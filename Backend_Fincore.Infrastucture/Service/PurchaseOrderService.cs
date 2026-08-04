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
            var purchaseOrder = await db.PurchaseOrder.Include(x => x.PurchaseOrderItems.Where(x => x.IsActive == 1))
                                        .FirstOrDefaultAsync(x => x.PurchaseOrderId == purchasedId && x.IsActive == 1);

            if (purchaseOrder == null)
            {
                throw new Exception("Purchase Order not found.");
            }

            if (purchaseOrder.Status == "Issued" || purchaseOrder.Status == "Completed")
            {
                throw new Exception("Issued or Completed Purchase Orders cannot be deleted.");
            }

            foreach (var item in purchaseOrder.PurchaseOrderItems)
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

        public async Task<List<PurchaseOrderDTO>> GetAllPurchasedOrder( PaginationDTO pagination)
        {

            var user = await db.User.Include(x => x.Role).FirstOrDefaultAsync(x => x.UserId == current.UserId);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            if (user.Role == null)
            {
                throw new Exception("Role not found.");
            }

            IQueryable<PurchaseOrder> query = db.PurchaseOrder.Include(x => x.Vendor).Include(x => x.Quotation).Where(x => x.IsActive == 1);

            switch (user.Role.RoleName)
            {
                case "Administrator":
                case "Procurement Manager":
                case "Warehouse Manager":
                case "Asset Manager":
                case "CFO":
                    
                    break;

                case "Vendor":
                                  query = query.Where(x => x.VendorId == user.MasterId);
                                  break;

                case "User":
                    throw new Exception("You are not authorized.");

                default:
                    throw new Exception("Invalid role.");
            }

            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                query = query.Where(x => x.PONumber.Contains(pagination.Search) ||
                                         x.Status.Contains(pagination.Search) ||
                                         x.Vendor.VendorName.Contains(pagination.Search));

            }

            var purchaseOrders = await query.OrderByDescending(x => x.CreatedAt)
                                            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                                            .Take(pagination.PageSize)
                                            .ToListAsync();


            return mapper.Map<List<PurchaseOrderDTO>>(purchaseOrders);

        }

        public async Task<PurchaseOrderDTO> GetPurchaseOrderById(int purchasedId)
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

            IQueryable<PurchaseOrder> query = db.PurchaseOrder.Include(x => x.Vendor)
                                                              .Include(x => x.Quotation)
                                                              .Where(x => x.IsActive == 1);

            switch (user.Role.RoleName)
            {
                case "Administrator":
                case "Procurement Manager":
                case "Warehouse Manager":
                case "Asset Manager":
                case "CFO":
                    // Can view any Purchase Order
                    break;

                case "Vendor":
                    query = query.Where(x => x.VendorId == user.MasterId);
                    break;

                case "User":
                    throw new Exception("You are not authorized.");

                default:
                    throw new Exception("Invalid role.");
            }

            var purchaseOrder = await query.FirstOrDefaultAsync(x => x.PurchaseOrderId == purchasedId);

            if (purchaseOrder == null)
            {
                throw new Exception("Purchase Order not found.");
            }

            return mapper.Map<PurchaseOrderDTO>(purchaseOrder);
        }


        
        public async Task UpdatePurchaseOrder(PurchaseOrderCUDTO Po, int id)
        {
            var purchaseOrder = await db.PurchaseOrder.Include(x => x.PurchaseOrderItems.Where(x => x.IsActive == 1))
                                      .FirstOrDefaultAsync(x => x.PurchaseOrderId == id && x.IsActive == 1);

            if (purchaseOrder == null)
            {
                throw new Exception("Purchase Order not found.");
            }

            if (purchaseOrder.Status == "Issued" || purchaseOrder.Status == "Completed")
            {
                throw new Exception("Issued or Completed Purchase Orders cannot be updated.");
            }

            
            bool poExists = await db.PurchaseOrder.AnyAsync(x => x.PONumber == Po.PONumber &&
                                                                 x.PurchaseOrderId != id &&
                                                                 x.IsActive == 1);


            if (poExists)
            {
                throw new Exception("Purchase Order Number already exists.");
            }

            
            bool quotationExists = await db.PurchaseOrder.AnyAsync(x => x.QuotationId == Po.QuotationId &&
                                                                        x.PurchaseOrderId != id &&
                                                                        x.IsActive == 1);

            if (quotationExists)
            {
                throw new Exception("Purchase Order already exists for this quotation.");
            }

           
            var quotation = await db.Quotation.Include(x => x.RFQVendor)
                           .FirstOrDefaultAsync(x => x.QuotationId == Po.QuotationId && x.IsActive == 1);


            if (quotation == null)
            {
                throw new Exception("Quotation not found.");
            }

          
            if (quotation.RFQVendor.VendorId != Po.VendorId)
            {
                throw new Exception("Selected vendor does not belong to the selected quotation.");
            }

            if (purchaseOrder.PurchaseOrderItems.Any())
            {
                if (purchaseOrder.QuotationId != Po.QuotationId || purchaseOrder.VendorId != Po.VendorId)
                {
                    throw new Exception("Quotation or Vendor cannot be changed because Purchase Order Items already exist.");
                }
            }

            purchaseOrder.PONumber = Po.PONumber;
            purchaseOrder.QuotationId = Po.QuotationId;
            purchaseOrder.VendorId = Po.VendorId;

            purchaseOrder.ModifiedBy = current.UserId;
            purchaseOrder.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            cache.Remove($"PO_{id}");

        }


        public async Task UpdatePOStatus(int purchaseOrderId, PurchasedOrderStatusDTO dto)
        {
            var purchaseOrder = await db.PurchaseOrder.Include(x => x.PurchaseOrderItems.Where(x => x.IsActive == 1))
                                     .FirstOrDefaultAsync(x => x.PurchaseOrderId == purchaseOrderId && x.IsActive == 1);

            if (purchaseOrder == null)
            {
                throw new Exception("Purchase Order not found.");
            }

           
            if (purchaseOrder.Status != "Draft")
            {
                throw new Exception("Only Draft Purchase Orders can be updated.");
            }

           
            if (dto.Status != "Issued" && dto.Status != "Cancelled")
            {
                throw new Exception("Purchase Order status can only be changed to Issued or Cancelled.");
            }

           
            if (dto.Status == "Issued")
            {
                if (!purchaseOrder.PurchaseOrderItems.Any())
                {
                    throw new Exception("Purchase Order must contain at least one item.");
                }

                if (purchaseOrder.TotalAmount <= 0)
                {
                    throw new Exception("Purchase Order total amount should be greater than zero.");
                }
            }

          
            if (dto.Status == "Cancelled")
            {
                foreach (var item in purchaseOrder.PurchaseOrderItems)
                {
                    item.Status = "Cancelled";
                    item.ModifiedBy = current.UserId;
                    item.ModifiedAt = DateTime.Now;
                }
            }

            purchaseOrder.Status = dto.Status;
            purchaseOrder.ModifiedBy = current.UserId;
            purchaseOrder.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            cache.Remove($"PO_{purchaseOrderId}");

        }

        public async Task<List<PurchaseOrderDTO>> FetchIssuedPO()
        {
            var data = await db.PurchaseOrder.Where(x => x.Status == "Issued" && x.IsActive == 1).ToListAsync();

            var res = mapper.Map<List<PurchaseOrderDTO>>(data);

            return res;

        }
    }
}
