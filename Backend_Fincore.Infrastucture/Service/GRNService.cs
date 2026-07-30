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
        public GRNService(AppDbContext db, IMapper mapper,ICurrentUserService current)
        {
            this.db = db;
            this.mapper = mapper;
            this.current = current;
        }

        public async Task AddGrn(GRNCUDTO grn)
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


            var grn = await db.GRN.Include(x => x.GRNItems).FirstOrDefaultAsync(x => x.GRNId == id && x.IsActive == 1);

            if (grn == null)
            {
                throw new Exception("GRN not found.");
            }

            if (grn.Status == "Received")
            {
                throw new Exception("Received GRN cannot be deleted.");
            }

            foreach (var item in grn.GRNItems.Where(x => x.IsActive == 1))
            {
                // Update PO Item Status
                var poItem = await db.PurchaseOrderItem.FirstOrDefaultAsync(x => x.POItemId == item.POItemId && x.IsActive == 1);


                if (poItem != null)
                {

                    decimal receivedQty = await db.GRNItem.Where(x => x.POItemId == item.POItemId && x.IsActive == 1 &&
                                                            x.GRNId != grn.GRNId && x.GRN.Status == "Received")
                                                             .SumAsync(x => x.Qty);


                    if (receivedQty == 0)
                    {
                        poItem.Status = "Pending";
                    }
                    else if (receivedQty < poItem.Qty)
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


                var po = await db.PurchaseOrder.Include(x => x.PurchaseOrderItems)
                                              .FirstOrDefaultAsync(x => x.PurchaseOrderId == grn.PurchaseOrderId);


                if (po != null)
                {
                    bool completed = po.PurchaseOrderItems.Where(x => x.IsActive == 1)
                                                          .All(x => x.Status == "Received");

                    po.Status = completed ? "Completed" : "Issued";
                }


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
                throw new Exception("user not found");
            }

            if (user.Role == null)
            {
                throw new Exception("Role not exists");
            }

            IQueryable<GRN> query = db.GRN.Include(x => x.ReceivedByUser).Include(x => x.PurchaseOrder)
                                     .ThenInclude(x => x.Vendor).Where(x => x.IsActive == 1);

            switch (user.Role.RoleName)
            {
                case "Administrator":
                case "Procurement Manager":
                case "Warehouse Manager":
                case "CFO":
                case "Asset Manager":
                    
                             break;

                case "Vendor":
                              query = query.Where(x => x.PurchaseOrder.VendorId == user.MasterId);
                              break;

                default:
                             throw new Exception("You are not authorized.");
            }


            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                query = query.Where(x => x.GRNNumber.Contains(pagination.Search) ||
                                         x.Status.Contains(pagination.Search) ||
                                         x.PurchaseOrder.PONumber.Contains(pagination.Search));

            }

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

            IQueryable<GRN> query = db.GRN.Include(x => x.PurchaseOrder).ThenInclude(x => x.Vendor)
                                       .Include(x => x.ReceivedByUser).Where(x => x.GRNId == id && x.IsActive == 1);


            switch (user.Role.RoleName)
            {
                case "Administrator":
                case "Procurement Manager":
                case "Warehouse Manager":
                case "CFO":
                case "Asset Manager":
                    
                    break;

                case "Vendor":
                    query = query.Where(x => x.PurchaseOrder.VendorId == user.MasterId);
                    break;

                default:
                    throw new Exception("You are not authorized.");
            }

            var grn = await query.FirstOrDefaultAsync();

            if (grn == null)
            {
                throw new Exception("GRN not found.");
            }

            return mapper.Map<GRNDTO>(grn);
        }

        public async Task UpdateGRN(GRNCUDTO grn, int id)
        {
            var data = await db.GRN.Include(x => x.GRNItems).FirstOrDefaultAsync(x => x.GRNId == id && x.IsActive == 1);


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
                throw new Exception("Purchase Order not found");
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


            if (grn.ReceivedDate > DateTime.Now)
            {
                throw new Exception("Received Date cannot be in the future.");
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


            mapper.Map(grn, data);


            data.ModifiedBy = current.UserId;


            data.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();
        }


        public async Task UpdateGRNStatus(int id, GrnStatusDTO dto)
        {
            var grn = await db.GRN.Include(x => x.GRNItems.Where(i => i.IsActive == 1))
                             .FirstOrDefaultAsync(x => x.GRNId == id && x.IsActive == 1);


            if (grn == null)
            {
                throw new Exception("GRN not found.");
            }

            if (grn.Status == "Received")
            {
                throw new Exception("GRN is already Received.");
            }

            if (dto.Status != "Received")
            {
                throw new Exception("GRN status can only be changed to Received.");
            }

            if (!grn.GRNItems.Any())
            {
                throw new Exception("Please add at least one GRN Item.");
            }

            // Validate quantity
            foreach (var grnItem in grn.GRNItems)
            {
                var poItem = await db.PurchaseOrderItem
                .FirstOrDefaultAsync(x => x.POItemId == grnItem.POItemId);

                decimal alreadyReceived = await db.GRNItem
                .Where(x => x.POItemId == grnItem.POItemId
                    && x.IsActive == 1
                    && x.GRNId != grn.GRNId
                    && x.GRN.Status == "Received")
                .SumAsync(x => x.Qty);

                decimal totalReceived = alreadyReceived + grnItem.Qty;

                if (totalReceived > poItem.Qty)
                {
                    throw new Exception($"Received quantity exceeds ordered quantity for {poItem.ItemName}");
                }
            }

          
            foreach (var grnItem in grn.GRNItems)
            {
                var poItem = await db.PurchaseOrderItem
                .FirstOrDefaultAsync(x => x.POItemId == grnItem.POItemId);

                decimal alreadyReceived = await db.GRNItem
                                                          .Where(x => x.POItemId == grnItem.POItemId
                                                              && x.IsActive == 1
                                                              && x.GRNId != grn.GRNId
                                                              && x.GRN.Status == "Received")
                                                          .SumAsync(x => x.Qty);

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

                poItem.ModifiedAt = DateTime.Now;
                poItem.ModifiedBy = current.UserId;
            }

            var purchaseOrder = await db.PurchaseOrder.Include(x => x.PurchaseOrderItems).FirstOrDefaultAsync(x => x.PurchaseOrderId == grn.PurchaseOrderId);

            bool completed = purchaseOrder.PurchaseOrderItems
                                          .Where(x => x.IsActive == 1)
                                          .All(x => x.Status == "Received");

            purchaseOrder.Status = completed ? "Completed" : "Issued";

            purchaseOrder.ModifiedAt = DateTime.Now;
            purchaseOrder.ModifiedBy = current.UserId;

            grn.Status = "Received";

            grn.ModifiedAt = DateTime.Now;
            grn.ModifiedBy = current.UserId;

            await db.SaveChangesAsync();
        }
    }
}
