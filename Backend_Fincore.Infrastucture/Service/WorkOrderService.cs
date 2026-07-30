using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.WorkOrder;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Infrastucture.Service 
{
    public class WorkOrderService : IWorkOrderService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly ICurrentUserService current;

        public WorkOrderService(AppDbContext db, IMapper mapper, ICurrentUserService current)
        {
            this.db = db;
            this.mapper = mapper;
            this.current = current;
        }


        public async Task<int> GetWorkOrderCount(PaginationDTO pagination)
        {
            int userId = current.UserId;

            var user = await db.User
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.IsActive == 1);

            if (user == null)
                throw new Exception("User not found.");

            if (user.Role == null)
                throw new Exception("User role not found.");

            IQueryable<WorkOrder> query = db.WorkOrder.Where(x => x.IsActive == 1);
            
            if (user.Role.RoleId == 1 || user.Role.RoleId == 2 )
            {
                // CFO sees all active work orders.
            }
            else
            {
                query = query.Where(x => x.CreatedBy == userId);
            }

            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                string searchText = pagination.Search.Trim();

                query = query.Where(x =>
                    x.WorkOrderId.ToString().Contains(searchText) ||
                    x.WorkOrderNumber.Contains(searchText) ||
                    x.Title.Contains(searchText) ||
                    x.Status.Contains(searchText));
            }

            return await query.CountAsync();
        }
        public async Task<List<WorkOrderReadDTO>> GetAll(PaginationDTO pagination)
        {
            int userId = current.UserId;

            if (pagination.PageNumber <= 0)
                pagination.PageNumber = 1;

            if (pagination.PageSize <= 0)
                pagination.PageSize = 10;

            var user = await db.User
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.IsActive == 1);

            if (user == null)
                throw new Exception("User not found or inactive.");

            if (user.Role == null)
                throw new Exception("User role not found.");

            IQueryable<WorkOrder> query = db.WorkOrder
                .Include(x => x.OpexRequest)
                .Include(x => x.Vendor)
                .Where(x => x.IsActive == 1);

            if (user.Role.RoleId == 1 || user.Role.RoleId == 2)
            {
                // CFO and Manager see all active work orders.
            }
            else
            {
                query = query.Where(x => x.CreatedBy == userId);
            }

            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                string searchText = pagination.Search.Trim();

                query = query.Where(x =>
                    x.WorkOrderId.ToString().Contains(searchText) ||
                    x.WorkOrderNumber.Contains(searchText) ||
                    x.Title.Contains(searchText) ||
                    x.Status.Contains(searchText));
            }

            var workOrders = await query
                .OrderByDescending(x => x.WorkOrderId)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return mapper.Map<List<WorkOrderReadDTO>>(workOrders);
        }
        public async Task<WorkOrderReadDTO?> GetById(int id)
        {
            var data = await db.WorkOrder
                .Include(x => x.OpexRequest)
                .Include(x => x.Vendor)
                .FirstOrDefaultAsync(x =>
                    x.WorkOrderId == id &&
                    x.IsActive == 1);

            if (data == null)
                throw new Exception("Work Order Not Found");

            return mapper.Map<WorkOrderReadDTO>(data);
        }
        public async Task<WorkOrderReadDTO> Create(WorkOrderWriteDTO dto)
        {
            bool workOrderNumberExists = await db.WorkOrder
                                    .AnyAsync(x =>
                                        x.WorkOrderNumber == dto.WorkOrderNumber &&
                                        x.IsActive == 1);

            if (workOrderNumberExists)
                throw new Exception("Work Order Number already exists.");

            var opexRequest = await db.OpexRequest
                .FirstOrDefaultAsync(x =>
                    x.OpexRequestId == dto.OpexRequestId &&
                    x.IsActive == 1);

            if (opexRequest == null)
                throw new Exception("OPEX Request not found or inactive.");

            if (opexRequest.Status != "Approved")
                throw new Exception("Only approved OPEX Request can be used.");

            var vendor = await db.Vendor
                .FirstOrDefaultAsync(x =>
                    x.VendorId == dto.VendorId &&
                    x.IsActive == 1);

            if (vendor == null)
                throw new Exception("Vendor not found or inactive.");

            if (dto.Amount <= 0)
                throw new Exception("Work Order amount must be greater than zero.");

            decimal usedAmount = await db.WorkOrder
                .Where(x =>
                    x.OpexRequestId == dto.OpexRequestId &&
                    x.Status != "Rejected" &&
                    x.Status != "Cancelled" &&
                    x.IsActive == 1)
                .SumAsync(x => x.Amount);

            decimal availableAmount = opexRequest.Amount - usedAmount;

            if (dto.Amount > availableAmount)
            {
                throw new Exception($"Work Order amount exceeds available OPEX amount of {availableAmount}.");
            }

            var workOrder = mapper.Map<WorkOrder>(dto);

            workOrder.Status = "Pending";
            workOrder.IsActive = 1;
            workOrder.CreatedAt = DateTime.Now;
            workOrder.CreatedBy = current.UserId;

            await db.WorkOrder.AddAsync(workOrder);
            await db.SaveChangesAsync();

            return mapper.Map<WorkOrderReadDTO>(workOrder);
        }

        public async Task<WorkOrderReadDTO> Update(int workOrderId, WorkOrderWriteDTO dto)
        {
            var workOrder = await db.WorkOrder.FirstOrDefaultAsync(x => x.WorkOrderId == workOrderId);

            if (workOrder == null)
                throw new Exception("Work Order not found.");

            if (workOrder.IsActive == 0)
                throw new Exception("Inactive Work Order cannot be updated.");

            if (workOrder.Status == "Approved")
                throw new Exception("Approved Work Order cannot be updated.");

            bool numberExists = await db.WorkOrder
                                .AnyAsync(x =>
                                    x.WorkOrderNumber == dto.WorkOrderNumber &&
                                    x.WorkOrderId != workOrderId &&
                                    x.IsActive == 1);

            if (numberExists)
                throw new Exception("Work Order Number already exists.");

            var opexRequest = await db.OpexRequest
                .FirstOrDefaultAsync(x =>
                    x.OpexRequestId == dto.OpexRequestId &&
                    x.IsActive == 1);

            if (opexRequest == null)
                throw new Exception("OPEX Request not found or inactive.");

            if (opexRequest.Status != "Approved")
                throw new Exception("Only approved OPEX Request can be used.");

            bool vendorExists = await db.Vendor
                .AnyAsync(x =>
                    x.VendorId == dto.VendorId &&
                    x.IsActive == 1);

            if (!vendorExists)
                throw new Exception("Vendor not found or inactive.");

            if (dto.Amount <= 0)
                throw new Exception("Work Order amount must be greater than zero.");

            decimal usedAmount = await db.WorkOrder
                                .Where(x =>
                                    x.OpexRequestId == dto.OpexRequestId &&
                                    x.WorkOrderId != workOrderId &&
                                    x.Status != "Rejected" &&
                                    x.Status != "Cancelled" &&
                                    x.IsActive == 1)
                                .SumAsync(x => x.Amount);

            decimal availableAmount = opexRequest.Amount - usedAmount;

            if (dto.Amount > availableAmount)
            {
                throw new Exception($"Work Order amount exceeds available OPEX amount of {availableAmount}.");
            }

            workOrder.OpexRequestId = dto.OpexRequestId;
            workOrder.WorkOrderNumber = dto.WorkOrderNumber;
            workOrder.VendorId = dto.VendorId;
            workOrder.Title = dto.Title;
            workOrder.Amount = dto.Amount;
            workOrder.StartDate = dto.StartDate;
            workOrder.EndDate = dto.EndDate;

            workOrder.ModifiedBy = current.UserId;
            workOrder.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            return mapper.Map<WorkOrderReadDTO>(workOrder);
        }

        public async Task<bool> Delete(int workOrderId)
        {
            var workOrder = await db.WorkOrder.FirstOrDefaultAsync(x => x.WorkOrderId == workOrderId);

            if (workOrder == null)
                throw new Exception("Work Order not found.");

            if (workOrder.IsActive == 0)
                throw new Exception("Work Order has already been deleted.");

            if (workOrder.Status == "Approved")
                throw new Exception("Approved Work Order cannot be deleted.");

            workOrder.IsActive = 0;
            workOrder.ModifiedBy = current.UserId;
            workOrder.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<WorkOrderReadDTO> Verify(int workOrderId, int approvedBy, WorkOrderVerifyDTO dto)
        {
            var workOrder = await db.WorkOrder
                .Include(x => x.OpexRequest)
                .FirstOrDefaultAsync(x =>
                    x.WorkOrderId == workOrderId);

            if (workOrder == null)
                throw new Exception("Work Order not found.");

            if (workOrder.IsActive == 0)
                throw new Exception("Inactive Work Order cannot be verified.");

            if (workOrder.Status == "Approved")
                throw new Exception("Work Order is already approved.");

            if (workOrder.Status == "Rejected")
                throw new Exception("Work Order is already rejected.");

            if (dto.Status != "Approved" && dto.Status != "Rejected")
            {
                throw new Exception("Status must be Approved or Rejected.");
            }

            var approver = await db.User
                .FirstOrDefaultAsync(x =>
                    x.UserId == approvedBy &&
                    x.IsActive == 1);

            if (approver == null)
                throw new Exception("Approver user not found or inactive.");

            if (dto.Status == "Approved")
            {
                if (workOrder.OpexRequest == null)
                {
                    throw new Exception("OPEX Request not found.");
                }

                if (workOrder.OpexRequest.IsActive == 0)
                {
                    throw new Exception("Inactive OPEX Request cannot be used.");
                }

                if (workOrder.OpexRequest.Status != "Approved")
                {
                    throw new Exception("Work Order cannot be approved because OPEX Request is not approved.");
                }

                decimal otherWorkOrderAmount = await db.WorkOrder
                    .Where(x =>
                        x.OpexRequestId == workOrder.OpexRequestId &&
                        x.WorkOrderId != workOrderId &&
                        x.Status == "Approved" &&
                        x.IsActive == 1)
                    .SumAsync(x => x.Amount);

                decimal availableAmount = workOrder.OpexRequest.Amount - otherWorkOrderAmount;

                if (workOrder.Amount > availableAmount)
                {
                    throw new Exception($"Work Order amount exceeds available OPEX amount of {availableAmount}.");
                }
            }

            workOrder.Status = "Approved";
            workOrder.ModifiedBy = approvedBy;
            workOrder.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            return mapper.Map<WorkOrderReadDTO>(workOrder);
        }

        public async Task<List<WorkOrderDropdownDTO>> GetDropdown()
        {
            var data = await db.WorkOrder
                .Where(x => x.IsActive == 1)
                .OrderBy(x => x.WorkOrderNumber)
                .Select(x => new WorkOrderDropdownDTO
                {
                    WorkOrderId = x.WorkOrderId,
                    WorkOrderNumber = x.WorkOrderNumber
                })
                .ToListAsync();

            return data;
        }
    }
}