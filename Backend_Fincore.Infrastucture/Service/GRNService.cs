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
            data.CreatedBy = current.UserId;

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

            IQueryable<GRN> query = db.GRN.Include(x => x.PurchaseOrder).AsQueryable();

            if (user.Role.RoleName == "User")
            {
                throw new Exception("You are not authorized.");
            }

            //Manager 
            else if (user.Role.RoleName == "Manager" || user.Role.RoleName == "HOD" || user.Role.RoleName == "Senior Manager")
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

            var purchaseOrder = await db.PurchaseOrder.FirstOrDefaultAsync(x => x.PurchaseOrderId == grn.PurchaseOrderId);


            if (purchaseOrder == null)
            {
                throw new Exception("Purchase Order not found");
            }

            bool exists = await db.GRN.AnyAsync(x => x.GRNNumber == grn.GRNNumber && x.GRNId != id);


            if (exists)
            {
                throw new Exception("GRN Number already exists.");
            }


            if (data.Status == "Received")
            {
                throw new Exception("Received GRN cannot be edited.");
            }


            if (purchaseOrder.Status != "Issued")
            {
                throw new Exception("Only Issued Purchase Orders can be linked to GRN.");
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


            mapper.Map<GRN>(data);


            data.ModifiedBy = current.UserId;
           

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

        
            if (dto.Status != "Draft" &&
                dto.Status != "Pending" &&
                dto.Status != "Received" &&
                dto.Status != "Rejected")
            {
                throw new Exception("Invalid GRN status.");
            }

            grn.Status = dto.Status;
            grn.ModifiedBy = current.UserId;
            grn.ModifiedAt = DateTime.Now;

            if (dto.Status == "Received")
            {
                foreach (var item in grn.GRNItems)
                {
                    var poItem = await db.PurchaseOrderItem.FirstOrDefaultAsync(x => x.POItemId == item.POItemId);

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
