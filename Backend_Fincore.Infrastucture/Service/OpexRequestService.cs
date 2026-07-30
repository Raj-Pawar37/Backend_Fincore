using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.OpexRequest;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Backend_Fincore.Infrastucture.Service
{
    public class OpexRequestService : IOpexRequestService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        private readonly ICurrentUserService current;

        public OpexRequestService(AppDbContext db, IMapper mapper, ICurrentUserService current)
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
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.IsActive == 1);

            if (user == null)
                throw new Exception("User not found or inactive.");

            if (user.Role == null)
                throw new Exception("User role not found.");

            IQueryable<OpexRequest> query = db.OpexRequest
                .Include(x => x.RequestedByUser)
                .Where(x => x.IsActive == 1);

            if (user.Role.RoleId == 1)
            {
                // CFO sees all active OPEX requests.
            }
            else if ( user.Role.RoleId == 2 || user.Role.RoleId == 4 || user.Role.RoleId == 5)
            {
                query = query.Where(x =>
                    x.RequestedByUser.IsActive == 1 &&
                    x.RequestedByUser.Username == user.Username);
            }
            else
            {
                query = query.Where(x => x.RequestedBy == userId);
            }

            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                string search = pagination.Search.Trim();

                query = query.Where(x => x.Status.Contains(search) || x.Title.Contains(search));
            }

            return await query.CountAsync();
        }
        public async Task<List<OpexRequestReadDTO>> GetAll(PaginationDTO pagination)
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
                throw new Exception("Logged-in user not found or inactive.");

            if (user.Role == null)
                throw new Exception("User role not found.");

            IQueryable<OpexRequest> query = db.OpexRequest
                .Include(x => x.RequestedByUser)
                .Include(x => x.ApprovedByUser)
                .Include(x => x.BudgetLine)
                .Where(x => x.IsActive == 1);

            if (user.Role.RoleId == 1)
            {
                // CFO sees all active OPEX requests.
            }
            else if (user.Role.RoleId == 2 || user.Role.RoleId == 4 || user.Role.RoleId == 5)
            {
                query = query.Where(x =>
                    x.RequestedByUser.IsActive == 1 &&
                    x.RequestedByUser.Username == user.Username);
            }
            else
            {
                query = query.Where(x => x.RequestedBy == userId);
            }

            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                string search = pagination.Search.Trim();

                query = query.Where(x =>
                    x.Status.Contains(search) ||
                    x.Title.Contains(search));
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
            var data = await db.OpexRequest
                .FirstOrDefaultAsync(x =>
                    x.OpexRequestId == id &&
                    x.IsActive == 1);

            if (data == null)
                return null;

            return mapper.Map<OpexRequestReadDTO>(data);
        }
        public async Task<OpexRequestReadDTO> Create(OpexRequestWriteDTO dto)
        {
            var budgetLine = await db.BudgetLine
                  .Include(x => x.Budget)
                  .FirstOrDefaultAsync(x =>
                  x.BudgetLineId == dto.BudgetLineId &&
                  x.IsActive == 1);

            if (budgetLine == null)
                throw new Exception("Budget Line not found or inactive.");

            if (budgetLine.Budget == null)
                throw new Exception("Budget not found.");

            if (budgetLine.Budget.ApprovedBy == null)
                throw new Exception("OPEX Request can only be created for an approved Budget.");

            if (dto.Amount <= 0)
                throw new Exception("Amount must be greater than zero.");

            decimal usedAmount = await db.OpexRequest
                .Where(x =>
                    x.BudgetLineId == dto.BudgetLineId &&
                    x.Status != "Rejected" &&
                    x.IsActive == 1)
                .SumAsync(x => x.Amount);

            decimal availableAmount = budgetLine.AllocatedAmount - usedAmount;

            if (dto.Amount > availableAmount)
            {
                throw new Exception($"Budget is not sufficient. Available amount is {availableAmount}.");
            }

            var opexRequest = mapper.Map<OpexRequest>(dto);

            opexRequest.CreatedBy = current.UserId;
            opexRequest.CreatedAt = DateTime.Now;
            opexRequest.IsActive = 1;
            opexRequest.RequestedBy = current.UserId;

            opexRequest.Status = "Pending";
            opexRequest.ApprovedBy = null;
            opexRequest.ApprovedDate = null;

            await db.OpexRequest.AddAsync(opexRequest);
            await db.SaveChangesAsync();

            return mapper.Map<OpexRequestReadDTO>(opexRequest);
        }




        public async Task<OpexRequestReadDTO> Update(int opexRequestId, OpexRequestWriteDTO dto)
        {
            var opexRequest = await db.OpexRequest.FirstOrDefaultAsync(x => x.OpexRequestId == opexRequestId);

            if (opexRequest == null)
                throw new Exception("OPEX Request not found.");

            if (opexRequest.IsActive == 0)
                throw new Exception("Inactive OPEX Request cannot be updated.");

            if (opexRequest.Status == "Approved")
                throw new Exception("Approved OPEX Request cannot be updated.");

            if (dto.Amount <= 0)
                throw new Exception("Amount must be greater than zero.");

            var budgetLine = await db.BudgetLine
                .FirstOrDefaultAsync(x =>
                    x.BudgetLineId == dto.BudgetLineId &&
                    x.IsActive == 1);

            if (budgetLine == null)
                throw new Exception("Budget Line not found or inactive.");

            decimal usedAmount = await db.OpexRequest
                .Where(x =>
                    x.BudgetLineId == dto.BudgetLineId &&
                    x.OpexRequestId != opexRequestId &&
                    x.Status != "Rejected" &&
                    x.IsActive == 1)
                .SumAsync(x => x.Amount);

            decimal availableAmount = budgetLine.AllocatedAmount - usedAmount;

            if (dto.Amount > availableAmount)
            {
                throw new Exception($"Budget is not sufficient. Available amount is {availableAmount}.");
            }

            opexRequest.BudgetLineId = dto.BudgetLineId;
            opexRequest.Title = dto.Title;

            opexRequest.Amount = dto.Amount;
            opexRequest.RequestedBy = current.UserId;

            opexRequest.ModifiedBy = current.UserId;
            opexRequest.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            return mapper.Map<OpexRequestReadDTO>(opexRequest);
        }

        public async Task<bool> Delete(int opexRequestId)
        {
            var opexRequest = await db.OpexRequest
                .FirstOrDefaultAsync(x => x.OpexRequestId == opexRequestId);

            if (opexRequest == null)
                throw new Exception("OPEX Request not found.");

            if (opexRequest.IsActive == 0)
                throw new Exception("OPEX Request has already been deleted.");

            if (opexRequest.Status == "Approved")
                throw new Exception("Approved OPEX Request cannot be deleted.");

            opexRequest.IsActive = 0;
            opexRequest.ModifiedBy = current.UserId;
            opexRequest.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            return true;
        }
        public async Task<OpexRequestReadDTO> Verify(int opexRequestId, int approvedBy, OpexRequestVerifyDTO dto)
        {
            var opexRequest = await db.OpexRequest.FirstOrDefaultAsync(x => x.OpexRequestId == opexRequestId);

            if (opexRequest == null)
                throw new Exception("OPEX Request not found.");

            if (opexRequest.IsActive == 0)
                throw new Exception("Inactive OPEX Request cannot be verified.");

            if (opexRequest.Status == "Approved")
                throw new Exception("OPEX Request is already approved.");

            if (opexRequest.Status == "Rejected")
                throw new Exception("OPEX Request is already rejected.");

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

            await using var transaction = await db.Database.BeginTransactionAsync();

          
                if (dto.Status == "Rejected")
                {
                    opexRequest.Status = "Rejected";
                    opexRequest.ApprovedBy = approvedBy;
                    opexRequest.ApprovedDate = DateTime.Now;

                    opexRequest.ModifiedBy = current.UserId;
                    opexRequest.ModifiedAt = DateTime.Now;

                    await db.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return mapper.Map<OpexRequestReadDTO>(opexRequest);
                }

                if (dto.VendorId == null)
                {
                    throw new Exception("Vendor is required to approve the OPEX Request.");
                }

                bool vendorExists = await db.Vendor
                    .AnyAsync(x =>
                        x.VendorId == dto.VendorId.Value &&
                        x.IsActive == 1);

                if (!vendorExists)
                    throw new Exception("Vendor not found or inactive.");

                bool workOrderExists = await db.WorkOrder
                    .AnyAsync(x =>
                        x.OpexRequestId == opexRequestId &&
                        x.IsActive == 1);

                if (workOrderExists)
                {
                    throw new Exception("Active Work Order already exists for this OPEX Request.");
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

                    IsActive = 1,
                    CreatedBy = current.UserId,
                    CreatedAt = DateTime.Now
                };

                await db.WorkOrder.AddAsync(workOrder);

                opexRequest.Status = "Approved";
                opexRequest.ApprovedBy = approvedBy;
                opexRequest.ApprovedDate = DateTime.Now;
                opexRequest.ModifiedBy = current.UserId;
                opexRequest.ModifiedAt = DateTime.Now;

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                return mapper.Map<OpexRequestReadDTO>(opexRequest);
          
        }
        public async Task<List<OpexRequestReadDTO>> SearchOpex(OpexSearchDTO dto)
        {
            IQueryable<OpexRequest> query = db.OpexRequest
                .Include(x => x.BudgetLine)
                .Include(x => x.RequestedByUser)
                .Include(x => x.ApprovedByUser)
                .Where(x => x.IsActive == 1);

            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                query = query.Where(x => x.Status == dto.Status);
            }

            // Department filter
            //if (!string.IsNullOrWhiteSpace(dto.Department))
            //{
            //    query = query.Where(x =>
            //        x.RequestedByUser.Department.DepartmentName == dto.Department);
            //}

            if (!string.IsNullOrWhiteSpace(dto.SearchText))
            {
                query = query.Where(x => x.Title.Contains(dto.SearchText));
            }

            query = query.OrderByDescending(x => x.OpexRequestId);

            if (string.IsNullOrWhiteSpace(dto.SearchText))
            {
                query = query.Take(20);
            }

            var opexRequests = await query.ToListAsync();

            return mapper.Map<List<OpexRequestReadDTO>>(opexRequests);
        }

        public async Task<List<OpexRequestDropdownDTO>> GetDropdown()
        {
            var data = await db.OpexRequest
             .Where(x => x.IsActive == 1 && x.Status == "Approved")
             .Select(x => new OpexRequestDropdownDTO
             {
                 OpexRequestId = x.OpexRequestId,
                 Title = x.Title
             })
             .ToListAsync();

            return data;
        }
    }

}