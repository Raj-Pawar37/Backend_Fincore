using AutoMapper;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs.GRN;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Service
{
    public class GRNService : IGRNService
    {
        private readonly AppDbContext db;

        IMapper mapper;
        public GRNService(AppDbContext db,IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task AddGrn(GRNCUDTO grn)
        {
            var purchsedOrder = await db.PurchaseOrder.FirstOrDefaultAsync
                          (x => x.PurchaseOrderId == grn.PurchaseOrderId);

            if (purchsedOrder == null)
            {
                throw new Exception("Purchase Order not found.");
            }

            var user = await db.User.FirstOrDefaultAsync(x => x.UserId == grn.ReceivedBy);

            if(user == null)
            {
                throw new Exception("User not found");
            }

            var data = mapper.Map<GRN>(grn);

            // Generate GRN Number
            int count = await db.GRN.CountAsync() + 1;
            data.GRNNumber = $"GRN{count:D4}";

            // Temporary until JWT is implemented
            //data.CreatedBy=userid
            data.CreatedBy = 1;

            await db.GRN.AddAsync(data);
            await db.SaveChangesAsync();


        }

        public async Task<List<GRNDTO>> GetAllGrns()
        {
            var res = await db.GRN.Include(x => x.PurchaseOrder)
                .Include(x => x.ReceivedByUser).ToListAsync();

            var data = mapper.Map<List<GRNDTO>>(res);

            return data;
        }

        public async Task<GRNDTO> GetGrnById(int id)
        {
            var grn = await db.GRN.Include(x => x.PurchaseOrder)
                           .Include(x => x.ReceivedByUser)
                          .FirstOrDefaultAsync(x => x.GRNId == id);

            if(grn != null)
            {
                var data = mapper.Map<GRNDTO>(grn);

                return data;
            }

            return null;
        }
    }
}
