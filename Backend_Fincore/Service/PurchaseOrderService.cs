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
    }
}
