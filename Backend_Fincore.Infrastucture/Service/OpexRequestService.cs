using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.OpexRequest;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;

using Backend_Fincore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Security.Claims;

namespace Backend_Fincore.Infrastucture.Service
{
    public class OpexRequestService : IOpexRequestService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
    
        private readonly ICurrentUserService current; 

        public OpexRequestService(AppDbContext db, IMapper mapper, ICurrentUserService current )
        {
            this.db = db;
            this.mapper = mapper;
            this.current = current;
        }

        public async Task<int> GetOpexRequestCount(PaginationDTO pagination)
        {
            int userId = current.UserId;

            var user = await db.User
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
                throw new Exception("User not found.");

            if (user.Role == null)
                throw new Exception("User role not found.");

            IQueryable<OpexRequest> query = db.OpexRequest
                .Include(x => x.RequestedByUser);

            if (user.Role.RoleId == 1)
            {
                // CFO sees all records
            }
            else if (user.Role.RoleId == 2 || user.Role.RoleId == 4 || user.Role.RoleId == 5)
            {
                query = query.Where(x =>
                    x.RequestedByUser.Username == user.Username);
            }
            else
            {
                query = query.Where(x => x.RequestedBy == userId);
            }

            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                query = query.Where(x =>
                    x.Status.Contains(pagination.Search) ||
                    x.Title.Contains(pagination.Search));
            }

            return await query.CountAsync();
        }
        public async Task<List<OpexRequestReadDTO>> GetAll(PaginationDTO pagination)
        {
            int userId = current.UserId;

            var user = await db.User
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
                throw new Exception("Logged-in user not found.");

            if (user.Role == null)
                throw new Exception("User role not found.");

            IQueryable<OpexRequest> query = db.OpexRequest
                .Include(x => x.RequestedByUser)
                .Include(x => x.ApprovedByUser)
                .Include(x => x.BudgetLine);

            if (user.Role.RoleId == 1)
            {
                // CFO sees every OPEX request.
                // No Where condition is required.
            }
            else if (user.Role.RoleId == 2 || user.Role.RoleId == 4 || user.Role.RoleId == 5)
            {
                query = query.Where(x =>
                    x.RequestedByUser.Username == user.Username);
            }
            else
            {
                query = query.Where(x =>
                    x.RequestedBy == userId);
            }

            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                query = query.Where(x =>
                    x.Status.Contains(pagination.Search) ||
                    x.Title.Contains(pagination.Search));
            }

            var opexRequests = await query
                .OrderByDescending(x => x.OpexRequestId)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return mapper.Map<List<OpexRequestReadDTO>>(opexRequests);
        }
        public async Task<OpexRequestReadDTO?> GetById(int id)
        {
            var data = await db.OpexRequest.FindAsync(id);

            if (data == null)
                return null;

            return mapper.Map<OpexRequestReadDTO>(data);
        }
        public async Task<OpexRequestReadDTO> Create(OpexRequestWriteDTO dto)
        {

         
       

            var budgetLine = await db.BudgetLine
                .FirstOrDefaultAsync(x => x.BudgetLineId == dto.BudgetLineId);

            if (budgetLine == null)
                throw new Exception("Budget Line not found.");

       
            if (dto.Amount <= 0)
                throw new Exception("Amount must be greater than zero.");

         
            decimal usedAmount = await db.OpexRequest
                .Where(x =>
                    x.BudgetLineId == dto.BudgetLineId &&
                    x.Status != "Rejected")
                .SumAsync(x => x.Amount);

            decimal availableAmount = budgetLine.AllocatedAmount - usedAmount;

            if (dto.Amount > availableAmount)
                throw new Exception($"Budget is not sufficient. Available amount is {availableAmount}.");

            var opexRequest = mapper.Map<OpexRequest>(dto);

          
            opexRequest.CreatedBy = current.UserId;
            opexRequest.CreatedAt = DateTime.Now;
            opexRequest.Status = "Pending";
            opexRequest.ApprovedBy = null;
            opexRequest.ApprovedDate = null;

            await db.OpexRequest.AddAsync(opexRequest);
            await db.SaveChangesAsync();

            return mapper.Map<OpexRequestReadDTO>(opexRequest);
        }


        //public async Task<OpexRequestReadDTO?> Update(
        //  int id,
        //  OpexRequestWriteDTO dto)
        //{
        //    var data = await db.OpexRequest.FindAsync(id);

        //    if (data == null)
        //        return null;

        //    if (data.Status != "Pending")
        //        throw new Exception(
        //            "Only Pending OPEX Request can be updated.");

        //    if (dto.Amount <= 0)
        //        throw new Exception("Amount must be greater than zero.");

        //    var budgetLine = await db.BudgetLine
        //        .FindAsync(dto.BudgetLineId);

        //    if (budgetLine == null)
        //        throw new Exception("Budget Line not found.");

        //    if (dto.Amount > budgetLine.AllocatedAmount)
        //        throw new Exception(
        //            "Requested amount exceeds available budget.");

        //    mapper.Map(dto, data);

        //    await db.SaveChangesAsync();

        //    return mapper.Map<OpexRequestReadDTO>(data);
        //}


        public async Task<OpexRequestReadDTO> Update(int opexRequestId,OpexRequestWriteDTO dto)
        {
            var opexRequest = await db.OpexRequest.FirstOrDefaultAsync(x => x.OpexRequestId == opexRequestId);
            //var userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);


            if (opexRequest == null)
                throw new Exception("OPEX Request not found.");

            if (opexRequest.Status == "Approved")
                throw new Exception("Approved OPEX Request cannot be updated.");
            if (dto.Amount <= 0)
                      throw new Exception("Amount must be greater than zero.");
            opexRequest.ModifiedAt = DateTime.Now;
            opexRequest.ModifiedBy=current.UserId;
            opexRequest.BudgetLineId = dto.BudgetLineId;
            opexRequest.Title = dto.Title;
            opexRequest.Amount = dto.Amount;
            opexRequest.RequestedBy = dto.RequestedBy;

            await db.SaveChangesAsync();

            return mapper.Map<OpexRequestReadDTO>(opexRequest);
        }

        public async Task<bool> Delete(int opexRequestId)
        {
            var opexRequest = await db.OpexRequest.FirstOrDefaultAsync(x => x.OpexRequestId == opexRequestId);

            if (opexRequest == null)
                throw new Exception("OPEX Request not found.");

            if (opexRequest.Status == "Approved")
                throw new Exception("Approved OPEX Request cannot be deleted.");

            db.OpexRequest.Remove(opexRequest);

            await db.SaveChangesAsync();

            return true;
        }
        public async Task<OpexRequestReadDTO> Verify(
            int opexRequestId,
            int approvedBy,
            OpexRequestVerifyDTO dto)
        {
            var opexRequest = await db.OpexRequest
                .FirstOrDefaultAsync(x =>
                    x.OpexRequestId == opexRequestId);

            if (opexRequest == null)
                throw new Exception("OPEX Request not found.");

            if (opexRequest.Status == "Approved")
                throw new Exception("OPEX Request is already approved.");

            if (opexRequest.Status == "Rejected")
                throw new Exception("OPEX Request is already rejected.");

            if (dto.Status != "Approved" &&
                dto.Status != "Rejected")
            {
                throw new Exception(
                    "Status must be Approved or Rejected.");
            }

            var approver = await db.User
                .FirstOrDefaultAsync(x => x.UserId == approvedBy);

            if (approver == null)
                throw new Exception("Approver user not found.");

            await using var transaction =
                await db.Database.BeginTransactionAsync();

            try
            {
                // Rejection does not create a Work Order
                if (dto.Status == "Rejected")
                {
                    opexRequest.Status = "Rejected";
                    opexRequest.ApprovedBy = approvedBy;
                    opexRequest.ApprovedDate = DateTime.Now;

                    await db.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return mapper.Map<OpexRequestReadDTO>(
                        opexRequest);
                }

                // Vendor is required when creating Work Order
                if (dto.VendorId == null)
                {
                    throw new Exception(
                        "Vendor is required to approve the OPEX Request.");
                }

                var vendorExists = await db.Vendor
                    .AnyAsync(x => x.VendorId == dto.VendorId.Value);

                if (!vendorExists)
                    throw new Exception("Vendor not found.");

                // Prevent duplicate Work Order
                bool workOrderExists = await db.WorkOrder
                    .AnyAsync(x =>
                        x.OpexRequestId == opexRequestId);

                if (workOrderExists)
                {
                    throw new Exception(
                        "Work Order already exists for this OPEX Request.");
                }

                var workOrder = new WorkOrder
                {
                    OpexRequestId = opexRequest.OpexRequestId,
                    WorkOrderNumber = $"WO-{DateTime.Now:yyyyMMddHHmmss}",
                    VendorId = dto.VendorId.Value,
                    Title = opexRequest.Title,
                    Amount = opexRequest.Amount,
                    StartDate = dto.StartDate ?? DateTime.Now,
                    EndDate = null,
                    Status = "Pending",
                    CreatedBy = current.UserId,
                    CreatedAt = DateTime.Now
                };

                await db.WorkOrder.AddAsync(workOrder);

                opexRequest.Status = "Approved";
                opexRequest.ApprovedBy = approvedBy;
                opexRequest.ApprovedDate = DateTime.Now;

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                return mapper.Map<OpexRequestReadDTO>(
                    opexRequest);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<OpexRequestReadDTO>> SearchOpex(OpexSearchDTO dto)
        {
            IQueryable<OpexRequest> query = db.OpexRequest
                .Include(x => x.BudgetLine)
                .Include(x => x.RequestedByUser)

                .Include(x => x.ApprovedByUser);


            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                query = query.Where(x =>
                    x.Status == dto.Status);
            }


            //if (!string.IsNullOrWhiteSpace(dto.Department))
            //{
            //    query = query.Where(x =>
            //        x.RequestedByUser.UserId
            //            .Contains(dto.Department));
            //}


            if (!string.IsNullOrWhiteSpace(dto.SearchText))
            {
                query = query.Where(x =>
                    x.Title.Contains(dto.SearchText));
            }

            query = query.OrderByDescending(x => x.OpexRequestId);

            if (string.IsNullOrWhiteSpace(dto.SearchText))
            {
                query = query.Take(20);
            }

            var opexRequests = await query.ToListAsync();

            return mapper.Map<List<OpexRequestReadDTO>>(opexRequests);
        }
    }

}