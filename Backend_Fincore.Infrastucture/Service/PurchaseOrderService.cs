using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.PurchaseOrderItem;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs.PurchaseOrder;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;


namespace Backend_Fincore.Service
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly AppDbContext db;

        IMapper mapper;
        public PurchaseOrderService(AppDbContext db, IMapper mapper)
        {
            this.db = db;

            this.mapper = mapper;
        }

        public async Task AddPurchaseOrderData(PurchaseOrderCUDTO PO)
        {
            var data = mapper.Map<PurchaseOrder>(PO);

            data.TotalAmount = 0;

            data.PONumber = "";

            //data.CreatedBy = userId
            data.CreatedBy = PO.CreatedBy;

            await db.PurchaseOrder.AddAsync(data);

            await db.SaveChangesAsync();

           
            var currentYear = DateTime.Now.Year;

            var lastPO = await db.PurchaseOrder.Where(x => x.CreatedAt.Year == currentYear)
                                  .OrderByDescending(x => x.PurchaseOrderId)
                                  .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastPO != null)
            {
                var parts = lastPO.PONumber.Split('-');
                nextNumber = int.Parse(parts[2]) + 1;
            }

            data.PONumber = $"AST-{currentYear}-{nextNumber:D4}";

            await db.SaveChangesAsync();
            
        }

        
        public async Task<bool> DeletePurchaseOrderById(int purchasedId)
        {
            var res = await  db.PurchaseOrder.FirstOrDefaultAsync(x => x.PurchaseOrderId == purchasedId);

            if(res != null)
            {
                db.PurchaseOrder.Remove(res);
                await db.SaveChangesAsync();

                return true;
            }

            return false;

        }

        public async Task<List<PurchaseOrderDTO>> GetAllPurchasedOrder()
        {
            var res = await db.PurchaseOrder.Include(x => x.Vendor)
                     .Include(x => x.Quotation).ToListAsync();

            var data = mapper.Map<List<PurchaseOrderDTO>>(res);

            return data;
        }

        public async Task<PurchaseOrderDTO> GetPurchaseOrderById(int purchasedId)
        {
            var res = await db.PurchaseOrder.Include(x => x.Vendor)
                     .Include(x => x.Quotation).FirstOrDefaultAsync(x => x.PurchaseOrderId == purchasedId);

            if (res != null)
            {
                var data = mapper.Map<PurchaseOrderDTO>(res);

                return data;

            }

            return null;

           
        }

        public async Task<List<QuotationDTO>> GetSelectedQuotation(string status)
        {
            var data = await db.Quotation.Where(x => x.Status == status).ToListAsync();

            var res = mapper.Map<List<QuotationDTO>>(data);

            return res;


        }

        public async Task UpdatePurchaseOrder(PurchaseOrderCUDTO Po, int id)
        {
            var purchasedOrder = await db.PurchaseOrder.FirstOrDefaultAsync(x => x.PurchaseOrderId == id);

            if(purchasedOrder != null)
            {
                purchasedOrder.VendorId = Po.VendorId;
                purchasedOrder.VendorId = Po.VendorId;
                purchasedOrder.QuotationId = Po.QuotationId;
            
                purchasedOrder.Status = Po.Status;

                purchasedOrder.ModifiedBy = Po.ModifiedBy;
                purchasedOrder.ModifiedAt = DateTime.Now;


                await db.SaveChangesAsync();
            }
        }

        public async Task CreatePOFromQuotation(SelectedQuotationDTO selectedQuotation)
        {

            var quotation = await db.Quotation.Include(x => x.RFQVendor)
                           .FirstOrDefaultAsync(x => x.QuotationId == selectedQuotation.QuotationId);


            if (quotation == null)
            {
                throw new Exception("Quotation not found.");
            }


            if (quotation.Status != "Accepted")
            {
                throw new Exception("Only accepted quotation can create Purchase Order.");
            }


            bool poExists = await db.PurchaseOrder.AnyAsync(x => x.QuotationId == selectedQuotation.QuotationId);


            if (poExists)
            {
                throw new Exception("Purchase Order already exists for this quotation.");
            }

            //var quotationItems = await db.QuotationItem.Where(x => x.QuotationId == selectedQuotation.QuotationId)
            //                          .ToListAsync();



            //if (!quotationItems.Any())
            //{
            //    throw new Exception("Quotation items not found.");
            //}


            int currentYear = DateTime.Now.Year;

            var lastPO = await db.PurchaseOrder.Where(x => x.CreatedAt.Year == currentYear)
                                               .OrderByDescending(x => x.PurchaseOrderId)
                                               .FirstOrDefaultAsync();


            int nextNumber = 1;

            if (lastPO != null)
            {
                string[] parts = lastPO.PONumber.Split('-');
                nextNumber = int.Parse(parts[2]) + 1;
            }

            string poNumber = $"PO-{currentYear}-{nextNumber:D4}";


            PurchaseOrder purchaseOrder = new PurchaseOrder
            {
                VendorId = quotation.RFQVendor.VendorId,
                QuotationId = quotation.QuotationId,
                PONumber = poNumber,
                TotalAmount = 0,
                Status = "Draft",
                CreatedBy = selectedQuotation.CreatedBy
            };

            await db.PurchaseOrder.AddAsync(purchaseOrder);
            await db.SaveChangesAsync();

            //foreach (var item in quotationItems)
            //{
            //    var poItem = mapper.Map<PurchaseOrderItem>(item);
            //    poItem.PurchaseOrderId = purchaseOrder.PurchaseOrderId;
            //    poItem.CreatedBy = selectedQuotation.CreatedBy;

            //    await db.PurchaseOrderItem.AddAsync(poItem);

            //}

            //await db.SaveChangesAsync();


            //var poItems = await db.PurchaseOrderItem.Where(x => x.PurchaseOrderId == purchaseOrder.PurchaseOrderId)
            //                    .ToListAsync();


            //decimal total = 0;

            //foreach (var item in poItems)
            //{
            //    decimal subTotal = item.Qty * item.UnitPrice;

            //    decimal tax = subTotal * (item.Tax ?? 0) / 100;

            //    decimal discount = subTotal * (item.Discount ?? 0) / 100;

            //    total += subTotal + tax - discount;
            //}

            //purchaseOrder.TotalAmount = total;

            //await db.SaveChangesAsync();

        }

        public async Task<List<PurchaseOrderDTO>> GetDepartmentPurchaseOrders(int userId)
        {
            var user = await db.User.FirstOrDefaultAsync(x => x.UserId == userId);

            if( user == null)
            {
                throw new Exception("User not found");
            }

            var employee = await db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == user.MasterId);

            if( employee == null)
            {
                throw new Exception("employee not found");

            }

            var employeeIds = await db.Employee.Where(x => x.DepartmentId == employee.DepartmentId)
                                    .Select(x => x.EmployeeId).ToListAsync();

            var userIds = await db.User.Where(x => employeeIds.Contains(x.MasterId))
                                .Select(x => x.UserId).ToListAsync();

            var purchaseOrders = await db.PurchaseOrder
                                      .Include(x => x.Vendor)
                                      .Include(x => x.Quotation)
                                      .Where(x => userIds.Contains(x.CreatedBy))
                                      .ToListAsync();

            var data = mapper.Map<List<PurchaseOrderDTO>>(purchaseOrders);

            return data;

        }

        public async Task UpdatePOStatus(int purchaseOrderId, UpdatePoStatusDTO dto)
        {
            var purchasedOrder = await db.PurchaseOrder.FirstOrDefaultAsync(x => x.PurchaseOrderId == purchaseOrderId);

            if (purchasedOrder == null) throw new Exception("Purchase Order not found.");


            if (purchasedOrder.Status != "Draft")
            {

                throw new Exception("Only Draft Purchase Orders can be Issued.");
            }

            purchasedOrder.Status = dto.Status;
            purchasedOrder.ModifiedBy = dto.ModifiedBy;


            await db.SaveChangesAsync();

        }

        public async Task<List<PurchaseOrderDTO>> GetVendorIssuedPurchaseOrders(int vendorId)
        {
            var purchasedOrders = await db.PurchaseOrder.Include(x => x.Vendor).Include(x => x.Quotation)
                                        .Where(x => x.VendorId == vendorId && x.Status == "Issued").ToListAsync();


            var data = mapper.Map<List<PurchaseOrderDTO>>(purchasedOrders);

            return data;

        }
    }
}
