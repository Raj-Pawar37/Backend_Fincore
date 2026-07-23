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

            var GRNName = await db.GRN.FirstOrDefaultAsync(x => x.GRNNumber == grn.GRNNumber);

            if(GRNName != null)
            {
                throw new Exception("Grn name already exists");
            }


            //var poItem = await db.PurchaseOrderItem.FirstOrDefaultAsync(x => x.POItemId == grn.POItemId);


            //if (poItem == null)
            //{
            //    throw new Exception("Purchase Order Item not found.");
            //}

            //if (poItem.Status == "Received")
            //{
            //    throw new Exception("Purchase Order Item has already been received.");
            //}



            if (purchsedOrder.Status != "Issued")
            {
                throw new Exception("Only Issued Purchase Orders can be added to GRN.");
            }

            var user = await db.User.FirstOrDefaultAsync(x => x.UserId == grn.ReceivedBy);

            if(user == null)
            {
                throw new Exception("User not found");
            }

            var data = mapper.Map<GRN>(grn);

            

            data.Status = "Draft";
            data.CreatedAt = DateTime.Now;
            data.CreatedBy = grn.ReceivedBy;





            await db.GRN.AddAsync(data);
            await db.SaveChangesAsync();


        }

        public async Task DeletegrnById(int id)
        {


            var grn = await db.GRN.Include(x => x.GRNItems).FirstOrDefaultAsync(x => x.GRNId == id);

            if (grn == null)
            {
                throw new Exception("GRN not found.");
            }

            if (grn.Status == "Received")
            {
                throw new Exception("Received GRN cannot be deleted.");
            }

            foreach (var item in grn.GRNItems)
            {
                var poItem = await db.PurchaseOrderItem.FirstOrDefaultAsync(x => x.POItemId == item.POItemId);

                if (poItem != null)
                {
                    //poItem.Status = "Not Recived"; 
                }
            }

            db.GRNItem.RemoveRange(grn.GRNItems);

           
            db.GRN.Remove(grn);

            await db.SaveChangesAsync();

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
            var data = await db.GRN.Include(x => x.GRNItems).FirstOrDefaultAsync(x => x.GRNId == id);


            if (data == null)
            {
                throw new Exception("GRN not found.");
            }
      
            var purchaseOrder = await db.PurchaseOrder.FirstOrDefaultAsync
                              (x => x.PurchaseOrderId == grn.PurchaseOrderId);

            if (purchaseOrder == null)
            {
                throw new Exception("Purchase Order not found");
            }

            bool exists = await db.GRN.AnyAsync(x => x.GRNNumber == grn.GRNNumber && x.GRNId != id);


            if (exists)
            {
                throw new Exception("GRN Number already exists.");
            }


            if (data.Status == "Received" && data.PurchaseOrderId != grn.PurchaseOrderId)
            {
                throw new Exception("Purchase Order cannot be changed after GRN is received.");
            }

            var user = await db.User.FirstOrDefaultAsync(x => x.UserId == grn.ReceivedBy);


            if (user == null)
            {
                throw new Exception("User not found.");
            }


            if (data.GRNItems.Any() && data.PurchaseOrderId != grn.PurchaseOrderId)

            {
                throw new Exception("Purchase Order cannot be changed because GRN Items already exist.");
            }

            data.PurchaseOrderId = grn.PurchaseOrderId;

            data.GRNNumber = grn.GRNNumber;
            
            data.ReceivedBy = grn.ReceivedBy;
            data.ReceivedDate = grn.ReceivedDate;
            data.Remarks = grn.Remarks;
            data.DeliveryChallanNumber = grn.DeliveryChallanNumber;

            

            // Temporary until JWT Authentication
            //data.ModifiedBy=userid
            data.ModifiedBy = grn.ModifiedBy;
            data.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();
        }
    }
}
