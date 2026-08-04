using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.GRN;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs.GRN;
using Backend_Fincore.DTOs.PurchaseOrderItem;

using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Infrastucture.Service
{
    public class GRNService : IGRNService
    {
        private readonly AppDbContext db;

        IMapper mapper;

        private readonly ICurrentUserService current;
        public GRNService(AppDbContext db, IMapper mapper, ICurrentUserService current)
        {
            this.db = db;
            this.mapper = mapper;
            this.current = current;
        }

        public async Task AddGrn(GRNCreate grn)
        {
            var purchsedOrder = await db.PurchaseOrder.FirstOrDefaultAsync(x => x.PurchaseOrderId == grn.PurchaseOrderId && x.IsActive == 1);

            if (purchsedOrder == null)
            {
                throw new Exception("Purchase Order not found.");
            }

            var GRNName = await db.GRN.FirstOrDefaultAsync(x => x.GRNNumber == grn.GRNNumber && x.IsActive == 1);

            if (GRNName != null)
            {
                throw new Exception("Grn name already exists");
            }

            if (purchsedOrder.Status != "Issued")
            {
                throw new Exception("Only Issued Purchase Orders can be added to GRN.");
            }

            var user = await db.User.FirstOrDefaultAsync(x => x.UserId == grn.ReceivedBy);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            bool draftExists = await db.GRN.AnyAsync(x => x.PurchaseOrderId == grn.PurchaseOrderId &&
                                                          x.Status == "Draft" &&
                                                          x.IsActive == 1);


            if (draftExists)
            {
                throw new Exception("Draft GRN already exists for this Purchase Order.");
            }

            var data = mapper.Map<GRN>(grn);


            data.Status = "Draft";
            data.CreatedAt = DateTime.Now;
            data.CreatedBy = current.UserId;

            await db.GRN.AddAsync(data);
            await db.SaveChangesAsync();


        }


        public async Task DeletegrnById(int id)
        {

            var grn = await db.GRN.Include(x => x.GRNItems.Where(x => x.IsActive == 1)).FirstOrDefaultAsync(x => x.GRNId == id && x.IsActive == 1);

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
                item.IsActive = 0;
                item.ModifiedBy = current.UserId;
                item.ModifiedAt = DateTime.Now;
            }


            grn.IsActive = 0;
            grn.ModifiedBy = current.UserId;
            grn.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

        }

        public async Task<int> GetAllGRNCount()
        {
            return await db.GRN.CountAsync(x => x.IsActive == 1);
        }

        public async Task<List<GRNDTO>> GetAllGrns(PaginationDTO pagination)
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

            IQueryable<GRN> query = db.GRN.Include(x => x.ReceivedByUser).Include(x => x.PurchaseOrder).ThenInclude(x => x.Vendor).Where(x => x.IsActive == 1);

            switch (user.Role.RoleName)
            {
                case "Administrator":
                case "Procurement Manager":
                case "Warehouse Manager":
                case "Asset Manager":
                case "CFO":
                    // Can view all GRNs
                    break;

                case "Vendor":
                    query = query.Where(x => x.PurchaseOrder.VendorId == user.MasterId);
                    break;

                case "User":
                    throw new Exception("You are not authorized.");

                default:
                    throw new Exception("Invalid role.");
            }



            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                query = query.Where(x => x.GRNNumber.Contains(pagination.Search) ||
                                         x.Status.Contains(pagination.Search) ||
                                         x.PurchaseOrder.PONumber.Contains(pagination.Search));

            }

            // Pagination
            var result = await query.OrderByDescending(x => x.CreatedAt)
                                    .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                                    .Take(pagination.PageSize)
                                    .ToListAsync();


            return mapper.Map<List<GRNDTO>>(result);
        }

        public async Task<GRNDTO> GetGrnById(int id)
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

            IQueryable<GRN> query = db.GRN.Include(x => x.PurchaseOrder).ThenInclude(x => x.Vendor).Include(x => x.ReceivedByUser)
                                 .Where(x => x.IsActive == 1);

            switch (user.Role.RoleName)
            {
                case "Administrator":
                case "Procurement Manager":
                case "Warehouse Manager":
                case "Asset Manager":
                case "CFO":

                    break;

                case "Vendor":
                    query = query.Where(x => x.PurchaseOrder.VendorId == user.MasterId);
                    break;


                case "User":
                    throw new Exception("You are not authorized.");

                default:
                    throw new Exception("Invalid role.");
            }

            var grn = await query.FirstOrDefaultAsync(x => x.GRNId == id);

            if (grn == null)
            {
                throw new Exception("GRN not found.");
            }

            return mapper.Map<GRNDTO>(grn);
        }

        public async Task UpdateGRN(GRNCUDTO grn, int id)
        {
            var data = await db.GRN.Include(x => x.GRNItems.Where(x => x.IsActive == 1)).FirstOrDefaultAsync(x => x.GRNId == id && x.IsActive == 1);

            if (data == null)
            {
                throw new Exception("GRN not found.");
            }

            if (data.Status == "Received")
            {
                throw new Exception("Received GRN cannot be edited.");
            }


            var purchaseOrder = await db.PurchaseOrder.FirstOrDefaultAsync(x => x.PurchaseOrderId == grn.PurchaseOrderId && x.IsActive == 1);


            if (purchaseOrder == null)
            {
                throw new Exception("Purchase Order not found.");
            }

            if (purchaseOrder.Status != "Issued")
            {
                throw new Exception("Only Issued Purchase Orders can be linked to GRN.");
            }


            bool exists = await db.GRN.AnyAsync(x => x.GRNNumber == grn.GRNNumber && x.GRNId != id && x.IsActive == 1);

            if (exists)
            {
                throw new Exception("GRN Number already exists.");
            }

            if (grn.ReceivedDate.Date > DateTime.Today)
            {
                throw new Exception("Received Date cannot be in the future.");
            }

            var receivedByUser = await db.User.FirstOrDefaultAsync(x => x.UserId == grn.ReceivedBy && x.IsActive == 1);

            if (receivedByUser == null)
            {
                throw new Exception("Received By user not found.");
            }


            if (data.GRNItems.Any() && data.PurchaseOrderId != grn.PurchaseOrderId)
            {
                throw new Exception("Purchase Order cannot be changed because GRN Items already exist.");
            }



            mapper.Map(grn, data);

            data.ModifiedBy = current.UserId;
            data.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();
        }


        public async Task UpdateGRNStatus(int id, GrnStatusDTO dto)
        {
            var grn = await db.GRN.Include(x => x.GRNItems.Where(x => x.IsActive == 1)).FirstOrDefaultAsync(x => x.GRNId == id && x.IsActive == 1);


            if (grn == null)
            {
                throw new Exception("GRN not found.");
            }

            if (grn.Status == "Received")
            {
                throw new Exception("GRN is already received.");
            }

            if (dto.Status != "Received")
            {
                throw new Exception("GRN status can only be changed to Received.");
            }

            if (!grn.GRNItems.Any())
            {
                throw new Exception("Please add at least one GRN Item.");
            }


            foreach (var grnItem in grn.GRNItems)
            {
                var poItem = await db.PurchaseOrderItem.FirstOrDefaultAsync(x => x.POItemId == grnItem.POItemId && x.IsActive == 1);


                if (poItem == null)
                {
                    throw new Exception("Purchase Order Item not found.");
                }

                decimal alreadyReceived = await db.GRNItem.Where(x =>
                                                                      x.POItemId == grnItem.POItemId &&
                                                                      x.IsActive == 1 &&
                                                                      x.GRNId != grn.GRNId &&
                                                                      x.GRN.IsActive == 1 &&
                                                                      x.GRN.Status == "Received")
                                                                .SumAsync(x => (decimal?)x.Qty) ?? 0;


                decimal totalReceived = alreadyReceived + grnItem.Qty;

                if (totalReceived > poItem.Qty)
                {
                    throw new Exception($"Received quantity exceeds ordered quantity for {poItem.ItemName}.");
                }
            }


            foreach (var grnItem in grn.GRNItems)
            {
                var poItem = await db.PurchaseOrderItem.FirstOrDefaultAsync(x => x.POItemId == grnItem.POItemId && x.IsActive == 1);


                decimal alreadyReceived = await db.GRNItem.Where(x =>
                                                                      x.POItemId == grnItem.POItemId &&
                                                                      x.IsActive == 1 &&
                                                                      x.GRNId != grn.GRNId &&
                                                                      x.GRN.IsActive == 1 &&
                                                                      x.GRN.Status == "Received")
                                                                     .SumAsync(x => (decimal?)x.Qty) ?? 0;


                decimal totalReceived = alreadyReceived + grnItem.Qty;

                if (totalReceived == 0)
                {
                    poItem.Status = "Pending";
                }
                else if (totalReceived < poItem.Qty)
                {
                    poItem.Status = "Partially Received";
                }
                else
                {
                    poItem.Status = "Received";
                }

                poItem.ModifiedBy = current.UserId;
                poItem.ModifiedAt = DateTime.Now;
            }

            var purchaseOrder = await db.PurchaseOrder.Include(x => x.PurchaseOrderItems.Where(x => x.IsActive == 1))
                                          .FirstOrDefaultAsync(x => x.PurchaseOrderId == grn.PurchaseOrderId && x.IsActive == 1);

            if (purchaseOrder == null)
            {
                throw new Exception("Purchase Order not found.");
            }

            bool completed = purchaseOrder.PurchaseOrderItems.All(x => x.Status == "Received");


            purchaseOrder.Status = completed ? "Completed" : "Issued";
            purchaseOrder.ModifiedBy = current.UserId;
            purchaseOrder.ModifiedAt = DateTime.Now;


            grn.Status = dto.Status;
            grn.ModifiedBy = current.UserId;
            grn.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();
        }

        public async Task<List<GRNDTO>> FetchDraftGRN()
        {
            return await db.GRN.Where(x => x.IsActive == 1 && x.Status == "Draft").Select(x => new GRNDTO
                                                                                  {
                                                                                      GRNId = x.GRNId,
                                                                                      GRNNumber = x.GRNNumber,
                                                                                      PurchaseOrderId = x.PurchaseOrderId,
                                                                                      PONumber = x.PurchaseOrder.PONumber
                                                                                  }).ToListAsync();




        }
    }
    
}
