using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.GRN;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs.GRN;
using Backend_Fincore.DTOs.PurchaseOrderItem;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Service
{
    public class GRNService : IGRNService
    {
        private readonly AppDbContext db;

        IMapper mapper;
        public GRNService(AppDbContext db, IMapper mapper)
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
                    poItem.Status = "Not Recived";
                }
            }

            db.GRNItem.RemoveRange(grn.GRNItems);


            db.GRN.Remove(grn);

            await db.SaveChangesAsync();

        }

        public async Task<int> GetAllGRNCount()
        {
            return await db.GRN.CountAsync();
        }

        public async Task<List<GRNDTO>> GetAllGrns(string masterType, int masterId, GrnStatusDTO dto,PaginationDTO pagination)
        {
            var search = db.GRN.AsQueryable();

            if (!string.IsNullOrEmpty(pagination.Search))
            {
                search = search.Where(x =>
                    x.GRNNumber.Contains(pagination.Search) ||

                    x.Status.Contains(pagination.Search)

                    );
            }

            var data = await search
                                   .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                                   .Take(pagination.PageSize)
                                   .ToListAsync();

            mapper.Map<PurchaseOrderItemDTO>(data);

            var query = db.GRN.Include(x => x.PurchaseOrder).AsQueryable();

            if (masterType == "Employee")
            {
                throw new Exception("You are not authorized to view GRNs.");
            }

            // Manager
            else if (masterType == "Manager")
            {
                var manager = await db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == masterId);

                if (manager == null)
                {
                    throw new Exception("Manager not found.");
                }

                // Employees of manager's department
                var employeeIds = await db.Employee.Where(x => x.DepartmentId == manager.DepartmentId).Select(x => x.EmployeeId)
                                         .ToListAsync();



                // UserIds of those employees
                var userIds = await db.User.Where(x => x.MasterType == "Employee" && employeeIds.Contains(x.MasterId))
                                  .Select(x => x.UserId).ToListAsync();



                query = query.Where(x => userIds.Contains(x.PurchaseOrder.CreatedBy));
            }

            // Vendor
            else if (masterType == "Vendor")
            {
                query = query.Where(x => x.PurchaseOrder.VendorId == masterId);
            }

            // CFO -> No filter

            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                query = query.Where(x => x.Status == dto.Status);
            }

            var res = await query.OrderByDescending(x => x.CreatedAt).ToListAsync();


            return mapper.Map<List<GRNDTO>>(res);
        }

        public async Task<GRNDTO> GetGrnById(int id)
        {
            var grn = await db.GRN.Include(x => x.PurchaseOrder)
                           .Include(x => x.ReceivedByUser)
                          .FirstOrDefaultAsync(x => x.GRNId == id);

            if (grn != null)
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
           

            data.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();
        }


        public async Task UpdateGRNStatus(int id, GrnStatusDTO dto)
        {
            var grn = await db.GRN.Include(x => x.GRNItems)
                              .FirstOrDefaultAsync(x => x.GRNId == id);

            if (grn == null)
            {
                throw new Exception("GRN not found.");
            }


            if (grn.Status == dto.Status)
            {
                throw new Exception($"GRN is already {dto.Status}.");
            }

            // Allow only valid statuses
            if (dto.Status != "Draft" &&
                dto.Status != "Pending" &&
                dto.Status != "Received" &&
                dto.Status != "Rejected")
            {
                throw new Exception("Invalid GRN status.");
            }

            grn.Status = dto.Status;
            grn.ModifiedBy = dto.userId;
            grn.ModifiedAt = DateTime.Now;

            if (dto.Status == "Received")
            {
                foreach (var item in grn.GRNItems)
                {
                    var poItem = await db.PurchaseOrderItem
                                         .FirstOrDefaultAsync(x => x.POItemId == item.POItemId);

                    if (poItem != null)
                    {
                        poItem.Status = "Received";
                    }
                }
            }


            await db.SaveChangesAsync();
        }
    }
}
