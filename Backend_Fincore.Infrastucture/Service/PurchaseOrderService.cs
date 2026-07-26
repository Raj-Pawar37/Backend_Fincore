using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.AccountMaster;
using Backend_Fincore.Application.DTOs.PurchaseOrder;

using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs.PurchaseOrder;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Reflection;


namespace Backend_Fincore.Infrastucture.Service
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly AppDbContext db;

        IMapper mapper;

        private readonly ICurrentUserService current;
        public PurchaseOrderService(AppDbContext db, IMapper mapper,ICurrentUserService current)
        {
            this.db = db;

            this.mapper = mapper;
            this.current = current;
        }

        public async Task AddPurchaseOrderData(PurchaseOrderCUDTO PO)
        {

            var quotation = await db.Quotation.Include(x => x.RFQVendor)
                                 .FirstOrDefaultAsync(x => x.QuotationId == PO.QuotationId);


            if (quotation == null)
            {
                throw new Exception("quotation not found");
            }

            if (quotation.RFQVendor.VendorId != PO.VendorId)
            {
                throw new Exception("Selected vendor does not belong to the selected quotation.");
            }

            var poExist = await db.PurchaseOrder.FirstOrDefaultAsync(x => x.QuotationId == PO.QuotationId);

            if (poExist != null)
            {
                throw new Exception("Purchsed order for this quotation is alaredy exists");
            }

        
            var poName = await db.PurchaseOrder.FirstOrDefaultAsync(x => x.PONumber == PO.PONumber);

            if (poName != null)
            {
                throw new Exception("Purchased order number or name alrady exists");
            }


            var quotationItems = await db.QuotationItem.Include(x => x.RFQItem)
                                .Where(x => x.QuotationId == PO.QuotationId).ToListAsync();

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

                decimal subTotal = PoItems.Qty * PoItems.UnitPrice;
                decimal tax = subTotal * (PoItems.Tax ?? 0) / 100;
                decimal discount = subTotal * (PoItems.Discount ?? 0) / 100;

                totalAmount += subTotal + tax - discount;

                await db.PurchaseOrderItem.AddAsync(PoItems);

                items.Status = "Selected";

            }

            purchaseOrder.TotalAmount = totalAmount;

            await db.SaveChangesAsync();


        }


        public async Task DeletePurchaseOrderById(int purchasedId)
        {
            var purchasePrder = await db.PurchaseOrder.FirstOrDefaultAsync(x => x.PurchaseOrderId == purchasedId);

            if (purchasePrder == null)
            {
                throw new Exception("Purchased order not exists");
            }

            var poItemsList = await db.PurchaseOrderItem.Where(x => x.PurchaseOrderId == purchasedId).ToListAsync();



            if (poItemsList.Any())
            {
                db.PurchaseOrderItem.RemoveRange(poItemsList);
            }


            db.PurchaseOrder.Remove(purchasePrder);
            await db.SaveChangesAsync();

        }

        public async Task<int> GetPurchasedOrderCount()
        {
            return await db.PurchaseOrder.CountAsync();
        }

        public async Task<List<PurchaseOrderDTO>> GetAllPurchasedOrder(PurchasedOrderFilterDTO pof,PaginationDTO pagination)
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

            IQueryable<PurchaseOrder> query = db.PurchaseOrder.Include(x => x.Vendor).Include(x => x.Quotation);

            if (user.Role.RoleName == "User" || user.Role.RoleName == "Employee")
            {
                throw new Exception("You are not authorized.");
            }

            //manager / senior manager/hod  filter

            else if (user.Role.RoleName =="Manager" || user.Role.RoleName == "HOD" || user.Role.RoleName == "Senior Manager")

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

                query = query.Where(x => userIds.Contains(x.CreatedBy));
            }

            // Vendor
            else if (user.Role.RoleName == "Vendor")
            {
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
                                        .Take(pagination.PageSize).ToListAsync();


            return mapper.Map<List<PurchaseOrderDTO>>(purchaseOrders);


        }   

        public async Task<PurchaseOrderDTO> GetPurchaseOrderById(int purchasedId)
        {
            var res = await db.PurchaseOrder.FirstOrDefaultAsync(x => x.PurchaseOrderId == purchasedId);


            if (res == null)
            {
                throw new Exception("Purchased order not exists");
            }


            var data = mapper.Map<PurchaseOrderDTO>(res);

            return data;

        }



        public async Task UpdatePurchaseOrder(PurchaseOrderCUDTO Po, int id)
        {
            var purchasedOrder = await db.PurchaseOrder.FirstOrDefaultAsync(x => x.PurchaseOrderId == id);

            if (purchasedOrder == null)
            {
                throw new Exception("Purchased order not exists");
            }

            bool quotationExist = await db.PurchaseOrder.AnyAsync(x => x.QuotationId == Po.QuotationId && x.PurchaseOrderId != id);



            if (quotationExist)
            {
                throw new Exception("Purchase Order already exists for this quotation.");
            }

            bool exists = await db.PurchaseOrder.AnyAsync(x => x.PONumber == Po.PONumber && x.PurchaseOrderId != id);


            if (exists)
            {
                throw new Exception("Purchase Order Number already exists.");
            }

            if (purchasedOrder.Status == "Completed")
            {
                throw new Exception("Completed Purchase Order cannot be updated.");
            }


            purchasedOrder.VendorId = Po.VendorId;
            purchasedOrder.QuotationId = Po.QuotationId;
            purchasedOrder.PONumber = Po.PONumber;
            purchasedOrder.ModifiedAt = DateTime.Now;
            purchasedOrder.Status = Po.Status;


            await db.SaveChangesAsync();

        }


        public async Task UpdatePOStatus(int purchaseOrderId, PurchasedOrderFilterDTO dto)
        {
            var purchasedOrder = await db.PurchaseOrder.FirstOrDefaultAsync(x => x.PurchaseOrderId == purchaseOrderId);

            if (purchasedOrder == null)
            {

                throw new Exception("Purchase Order not found.");
            }


            if (purchasedOrder.Status != "Draft")
            {

                throw new Exception("Only Draft Purchase Orders can be Issued.");
            }

            if (dto.Status != "Issued")
            {
                throw new Exception("Purchase Order status can only be changed to Issued.");
            }


            bool hasItems = await db.PurchaseOrderItem.AnyAsync(x => x.PurchaseOrderId == purchaseOrderId);

            if (!hasItems)
            {
                throw new Exception("Purchase Order must contain at least one item.");
            }

            if (purchasedOrder.TotalAmount <= 0)
            {
                throw new Exception("Purchase Order Total Amount should be greater than zero.");
            }

            purchasedOrder.Status = dto.Status;
            purchasedOrder.ModifiedBy = current.UserId;
            purchasedOrder.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

        }

       

       
    }
}
