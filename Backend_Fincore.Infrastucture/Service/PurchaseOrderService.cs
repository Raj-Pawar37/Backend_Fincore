using AutoMapper;
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
            data.CreatedBy = 1;

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

        public async Task UpdatePurchaseOrder(PurchaseOrderCUDTO Po, int id)
        {
            var purchasedOrder = await db.PurchaseOrder.FirstOrDefaultAsync(x => x.PurchaseOrderId == id);

            if(purchasedOrder != null)
            {
                purchasedOrder.VendorId = Po.VendorId;
                purchasedOrder.VendorId = Po.VendorId;
                purchasedOrder.QuotationId = Po.QuotationId;
            
                purchasedOrder.Status = Po.Status;

                purchasedOrder.ModifiedBy = 1;
                purchasedOrder.ModifiedAt = DateTime.Now;


                await db.SaveChangesAsync();
            }
        }
    }
}
