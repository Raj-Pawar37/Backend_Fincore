using AutoMapper;
using Backend_Fincore.Application.DTOs.WorkOrder;
using Backend_Fincore.Data;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Services
{
    public class WorkOrderService : IWorkOrderService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public WorkOrderService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<List<WorkOrderReadDTO>> GetAll(int userId)
        {
            var user = await db.User
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
                throw new Exception("User not found.");

            if (user.Role == null)
                throw new Exception("User role not found.");

            IQueryable<WorkOrder> query = db.WorkOrder
                .Include(x => x.OpexRequest)
                .Include(x => x.Vendor);
            //.Include(x => x.CreatedByUser)
            //.Include(x => x.ApprovedByUser);

            if (user.Role.RoleName == "CFO")
            {
                // CFO sees all work orders
            }
            else if (user.Role.RoleName == "Manager")
            {
                query = query.Where(x =>
                    x.CreatedBy ==
                    user.RoleId);
            }
            else
            {
                query = query.Where(x =>
                    x.CreatedBy == userId);
            }

            var workOrders = await query
                .OrderByDescending(x => x.WorkOrderId)
                .ToListAsync();

            return mapper.Map<List<WorkOrderReadDTO>>(
                workOrders);
        }

        public async Task<WorkOrderReadDTO?> GetById(int id)
        {
            var data = await db.WorkOrder
                .Include(x => x.OpexRequest)
                .Include(x => x.Vendor)
                .FirstOrDefaultAsync(x => x.WorkOrderId == id);

            if (data == null)
                return null;

            return mapper.Map<WorkOrderReadDTO>(data);
        }

        public async Task<WorkOrderReadDTO> Create(
      WorkOrderWriteDTO dto)
        {
          


            bool workOrderNumberExists = await db.WorkOrder
                .AnyAsync(x =>
                    x.WorkOrderNumber == dto.WorkOrderNumber);

            if (workOrderNumberExists)
                throw new Exception(
                    "Work Order Number already exists.");

            var opexRequest = await db.OpexRequest
                .FirstOrDefaultAsync(x =>
                    x.OpexRequestId == dto.OpexRequestId);

            if (opexRequest == null)
                throw new Exception("OPEX Request not found.");

            if (opexRequest.Status != "Approved")
                throw new Exception(
                    "Only approved OPEX Request can be used.");

            var vendor = await db.Vendor
                .FirstOrDefaultAsync(x =>
                    x.VendorId == dto.VendorId);

            if (vendor == null)
                throw new Exception("Vendor not found.");

            if (dto.Amount <= 0)
                throw new Exception(
                    "Work Order amount must be greater than zero.");

            decimal usedAmount = await db.WorkOrder
                .Where(x =>
                    x.OpexRequestId == dto.OpexRequestId &&
                    x.Status != "Rejected" &&
                    x.Status != "Cancelled")
                .SumAsync(x => x.Amount);

            decimal availableAmount =
                opexRequest.Amount - usedAmount;

            if (dto.Amount > availableAmount)
            {
                throw new Exception(
                    $"Work Order amount exceeds available OPEX amount of {availableAmount}.");
            }

            var workOrder = mapper.Map<WorkOrder>(dto);

            workOrder.Status = "Pending";
            workOrder.CreatedAt = DateTime.Now;
            //workOrder.CreatedBy = approver;

            await db.WorkOrder.AddAsync(workOrder);
            await db.SaveChangesAsync();

            return mapper.Map<WorkOrderReadDTO>(workOrder);
        }

        public async Task<WorkOrderReadDTO> Update(
          int workOrderId,
          WorkOrderWriteDTO dto)
        {
            var workOrder = await db.WorkOrder
                .FirstOrDefaultAsync(x =>
                    x.WorkOrderId == workOrderId);

            if (workOrder == null)
                throw new Exception("Work Order not found.");

            if (workOrder.Status == "Approved")
                throw new Exception(
                    "Approved Work Order cannot be updated.");

            bool numberExists = await db.WorkOrder
                .AnyAsync(x =>
                    x.WorkOrderNumber == dto.WorkOrderNumber &&
                    x.WorkOrderId != workOrderId);

            if (numberExists)
                throw new Exception(
                    "Work Order Number already exists.");

            var opexRequest = await db.OpexRequest
                .FirstOrDefaultAsync(x =>
                    x.OpexRequestId == dto.OpexRequestId);

            if (opexRequest == null)
                throw new Exception("OPEX Request not found.");

            if (opexRequest.Status != "Approved")
                throw new Exception(
                    "Only approved OPEX Request can be used.");

            var vendorExists = await db.Vendor
                .AnyAsync(x => x.VendorId == dto.VendorId);

            if (!vendorExists)
                throw new Exception("Vendor not found.");

            if (dto.Amount <= 0)
                throw new Exception(
                    "Work Order amount must be greater than zero.");

            decimal usedAmount = await db.WorkOrder
                .Where(x =>
                    x.OpexRequestId == dto.OpexRequestId &&
                    x.WorkOrderId != workOrderId &&
                    x.Status != "Rejected" &&
                    x.Status != "Cancelled")
                .SumAsync(x => x.Amount);

            decimal availableAmount =
                opexRequest.Amount - usedAmount;

            if (dto.Amount > availableAmount)
            {
                throw new Exception(
                    $"Work Order amount exceeds available OPEX amount of {availableAmount}.");
            }

            workOrder.OpexRequestId = dto.OpexRequestId;
            workOrder.WorkOrderNumber = dto.WorkOrderNumber;
            workOrder.VendorId = dto.VendorId;
            workOrder.Title = dto.Title;
            workOrder.Amount = dto.Amount;
            workOrder.StartDate = dto.StartDate;
            workOrder.EndDate = dto.EndDate;

            await db.SaveChangesAsync();

            return mapper.Map<WorkOrderReadDTO>(workOrder);
        }

        public async Task<bool> Delete(int workOrderId)
        {
            var workOrder = await db.WorkOrder
                .FirstOrDefaultAsync(x =>
                    x.WorkOrderId == workOrderId);

            if (workOrder == null)
                throw new Exception("Work Order not found.");

            if (workOrder.Status == "Approved")
                throw new Exception(
                    "Approved Work Order cannot be deleted.");

            db.WorkOrder.Remove(workOrder);

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<WorkOrderReadDTO> Verify(int workOrderId,int approvedBy,WorkOrderVerifyDTO dto)
        {
            var workOrder = await db.WorkOrder
                .Include(x => x.OpexRequest)
                .FirstOrDefaultAsync(x =>
                    x.WorkOrderId == workOrderId);

            if (workOrder == null)
                throw new Exception("Work Order not found.");

            if (workOrder.Status == "Approved")
                throw new Exception(
                    "Work Order is already approved.");

            if (dto.Status != "Approved" &&
                dto.Status != "Rejected")
            {
                throw new Exception(
                    "Status must be Approved or Rejected.");
            }

            var approver = await db.User
                .FirstOrDefaultAsync(x =>
                    x.UserId == approvedBy);

            if (approver == null)
                throw new Exception("Approver user not found.");

            if (dto.Status == "Approved")
            {
                if (workOrder.OpexRequest == null)
                    throw new Exception("OPEX Request not found.");

                if (workOrder.OpexRequest.Status != "Approved")
                {
                    throw new Exception(
                        "Work Order cannot be approved because OPEX Request is not approved.");
                }

                decimal otherWorkOrderAmount =
                    await db.WorkOrder
                        .Where(x =>
                            x.OpexRequestId ==
                                workOrder.OpexRequestId &&
                            x.WorkOrderId != workOrderId &&
                            x.Status == "Approved")
                        .SumAsync(x => x.Amount);

                decimal availableAmount =
                    workOrder.OpexRequest.Amount -
                    otherWorkOrderAmount;

                if (workOrder.Amount > availableAmount)
                {
                    throw new Exception(
                        $"Work Order amount exceeds available OPEX amount of {availableAmount}.");
                }
            }

            workOrder.Status = dto.Status;
            workOrder.ModifiedBy = approvedBy;
            workOrder.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            return mapper.Map<WorkOrderReadDTO>(workOrder);
        }
    }
}