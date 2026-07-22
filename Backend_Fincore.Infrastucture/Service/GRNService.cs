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
            var purchsedOrder = await db.PurchaseOrder.FirstOrDefaultAsync(x => x.PurchaseOrderId == grn.PurchaseOrderId);



            if (purchsedOrder == null)
            {
                throw new Exception("Purchase Order not found.");
            }

            if(purchsedOrder.Status != "Issued")
            {
                throw new Exception("Only Issued Purchase Orders can be added to GRN.");
            }

            var user = await db.User.FirstOrDefaultAsync(x => x.UserId == grn.ReceivedBy);

            if(user == null)
            {
                throw new Exception("User not found");
            }

            var data = mapper.Map<GRN>(grn);

           

            var currentYear = DateTime.Now.Year;

            var lastGRN = await db.GRN.Where(x => x.CreatedAt.Year == currentYear)
                                  .OrderByDescending(x => x.GRNId)
                                  .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastGRN != null)
            {
                var parts = lastGRN.GRNNumber.Split('-');
                nextNumber = int.Parse(parts[2]) + 1;
            }

            data.GRNNumber = $"GRN-{currentYear}-{nextNumber:D4}";

            // Temporary until JWT get implemented
            //data.CreatedBy=userid
            data.CreatedBy = grn.CreatedBy;

            

            await db.GRN.AddAsync(data);
            await db.SaveChangesAsync();


        }

        public async Task<bool> DeletegrnById(int id)
        {
            var data = await db.GRN.FirstOrDefaultAsync(x => x.GRNId == id);

            if(data != null)
            {
                db.GRN.Remove(data);
                await db.SaveChangesAsync();

                return true;
            }

            return false;

        }

        public async Task<List<GRNDTO>> GetAllGrns()
        {
            var res = await db.GRN.Include(x => x.PurchaseOrder)
                .Include(x => x.ReceivedByUser).Where(x=> x.PurchaseOrder.Status == "Issued").ToListAsync();


            

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

        public async Task UpdateGRN(GRNCUDTO grn, int id)
        {
            var data = await db.GRN.FirstOrDefaultAsync(x => x.GRNId == id);


            if (data == null)
            {
                throw new Exception("GRN not found.");
            }
      
            var purchaseOrder = await db.PurchaseOrder.FirstOrDefaultAsync
                              (x => x.PurchaseOrderId == grn.PurchaseOrderId);

            if (purchaseOrder == null)
            {
                throw new Exception("Purchase Order not found.");
            }

            var user = await db.User.FirstOrDefaultAsync(x => x.UserId == grn.ReceivedBy);


            if (user == null)
            {
                throw new Exception("User not found.");
            }

            data.PurchaseOrderId = grn.PurchaseOrderId;
            data.ReceivedBy = grn.ReceivedBy;
            data.ReceivedDate = grn.ReceivedDate;
            data.Remarks = grn.Remarks;
            data.DeliveryChallanNumber = grn.DeliveryChallanNumber;
            data.Status = grn.Status;

            // Temporary until JWT Authentication
            //data.ModifiedBy=userid
            data.ModifiedBy = grn.ModifiedBy;
            data.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();
        }
    }
}
