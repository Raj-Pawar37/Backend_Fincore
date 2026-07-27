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
                var poItem = await db.PurchaseOrderItem
                              .FirstOrDefaultAsync(x => x.POItemId == item.POItemId && x.IsActive == 1);

                if (poItem != null)
                {
                    poItem.Status = "Pending";
                    poItem.ModifiedBy = current.UserId;
                    poItem.ModifiedAt = DateTime.Now;
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
            return await db.GRN.CountAsync();
        }

        public async Task<List<GRNDTO>> GetAllGrns(GrnStatusDTO dto,PaginationDTO pagination)
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

            IQueryable<GRN> query = db.GRN.Include(x => x.PurchaseOrder).ThenInclude(x => x.Vendor).Where(x => x.IsActive == 1).AsQueryable();

            if (user.Role.RoleName == "User" || user.Role.RoleName == "Employee")
            {
                throw new Exception("You are not authorized.");
            }

            //Manager 
            else if (user.Role.RoleName == "Manager" || user.Role.RoleName == "HOD" || user.Role.RoleName == "Senior Manager")
            {

                var employee = await db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == user.MasterId && x.IsActive == 1);

                if (employee == null)
                {
                    throw new Exception("Employee not found");
                }

                var empIds = await db.Employee.Where(x => x.DepartmentId == employee.DepartmentId && x.IsActive == 1)
                                   .Select(x => x.EmployeeId).ToListAsync();

                var userIds = await db.User.Where(x => x.MasterType == "Employee" && empIds
                                    .Contains(x.MasterId) && (x.Role.RoleName == "Manager"
                                    || x.Role.RoleName == "HOD" || x.Role.RoleName == "Senior Manager"))
                                    .Select(x => x.UserId).ToListAsync();

                query = query.Where(x => userIds.Contains(x.CreatedBy));

            }


            // Vendor
            else if (user.Role.RoleName == "Vendor")
            {
                query = query.Where(x => x.PurchaseOrder.VendorId == user.MasterId);
            }

            else if (user.Role.RoleName == "CFO")
            {

            }
            else
            {
                throw new Exception("Invalid role.");
            }

            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                query = query.Where(x => x.Status == dto.Status);
            }

            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                query = query.Where(x =>
                    x.GRNNumber.Contains(pagination.Search) ||
                    x.Status.Contains(pagination.Search));
            }

            // Pagination

            var result = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();


            return mapper.Map<List<GRNDTO>>(result);
        }

        public async Task<GRNDTO> GetGrnById(int id)
        {
            var grn = await db.GRN.Include(x => x.PurchaseOrder)
                           .Include(x => x.ReceivedByUser)
                          .FirstOrDefaultAsync(x => x.GRNId == id && x.IsActive == 1);

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
                throw new Exception("At least one GRN Item is required before marking the GRN as Received.");
            }

           
            grn.Status = "Received";
            grn.ModifiedBy = current.UserId;
            grn.ModifiedAt = DateTime.Now;

           
            foreach (var item in grn.GRNItems)
            {
                var poItem = await db.PurchaseOrderItem
                    .FirstOrDefaultAsync(x => x.POItemId == item.POItemId && x.IsActive == 1);

                if (poItem != null)
                {
                    poItem.Status = "Received";
                    poItem.ModifiedBy = current.UserId;
                    poItem.ModifiedAt = DateTime.Now;
                }
            }

            
            var purchaseOrder = await db.PurchaseOrder
                .FirstOrDefaultAsync(x => x.PurchaseOrderId == grn.PurchaseOrderId && x.IsActive == 1);

            if (purchaseOrder != null)
            {
                bool allReceived = await db.PurchaseOrderItem
                    .Where(x => x.PurchaseOrderId == purchaseOrder.PurchaseOrderId && x.IsActive == 1)
                    .AllAsync(x => x.Status == "Received");

                if (allReceived)
                {
                    purchaseOrder.Status = "Completed";
                    purchaseOrder.ModifiedBy = current.UserId;
                    purchaseOrder.ModifiedAt = DateTime.Now;
                }
            }

            await db.SaveChangesAsync();
        }
    }
}
