using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;


namespace Backend_Fincore.Infrastucture.Service
{
    public class CapexRequestService : ICapexRequestService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly ICurrentUserService currentUser;
        public CapexRequestService(AppDbContext db,IMapper mapper, ICurrentUserService currentUser)
        {
            this.db = db;
            this.mapper = mapper;
            this.currentUser = currentUser;
        }
        public async Task<List<CapexVerifyDropdownDTO>>
    GetCapexVerifyDropdown(string? searchText)
        {
            var approver = await db.User
                .FirstOrDefaultAsync(x =>
                    x.UserId == currentUser.UserId &&
                    x.IsActive == 1);

            if (approver == null)
            {
                throw new Exception("Approver not found.");
            }

            var query =
                from capex in db.CapexRequest
                join approval in db.Approval
                    on approver.RoleId equals approval.RoleId
                where
                    capex.IsActive == 1 &&
                    capex.Status == "Pending" &&
                    approval.IsActive == 1 &&
                    capex.Amount >= approval.MinAmount &&
                    capex.Amount <= approval.MaxAmount &&
                    capex.BudgetLine.IsActive == 1 &&
                    capex.BudgetLine.Budget.IsActive == 1 &&
                    capex.BudgetLine.Budget.ApprovedBy != null &&
                    capex.BudgetLine.Budget.ApprovedDate != null
                select capex;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                string keyword = searchText.Trim();

                query = query.Where(x =>
                    x.Title.Contains(keyword) ||
                    x.CapexRequestId.ToString().Contains(keyword) ||
                    x.RequestedByUser.Username.Contains(keyword));
            }

            return await query
                .OrderByDescending(x => x.CapexRequestId)
                .Take(20)
                .Select(x => new CapexVerifyDropdownDTO
                {
                    CapexRequestId = x.CapexRequestId,

                    DisplayName =
                        "CAPEX-" + x.CapexRequestId +
                        " - " + x.Title,

                    Amount = x.Amount,

                    RequestedByName =
                        x.RequestedByUser.Username
                })
                .ToListAsync();
        }

        public async Task<CapexReadDTO> AddCapexRequest(CapexWriteDTO dto)
        {
            var budgetLine = await db.BudgetLine
                .FirstOrDefaultAsync(x =>
                    x.BudgetLineId == dto.BudgetLineId &&
    x.IsActive == 1);

            if (budgetLine == null)
            {
                throw new Exception("Budget line not found.");
            }

            var user = await db.User
                .FirstOrDefaultAsync(x =>
                    x.UserId == currentUser.UserId && x.IsActive == 1);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            if (dto.Amount <= 0)
            {
                throw new Exception(
                    "CAPEX amount must be greater than zero.");
            }

            decimal approvedAmount = await db.CapexRequest
                .Where(x => x.BudgetLineId == dto.BudgetLineId && x.Status == "Approved" && x.IsActive == 1)
                .SumAsync(x => (decimal?)x.Amount) ?? 0;

            decimal availableAmount =
                budgetLine.AllocatedAmount - approvedAmount;

            if (dto.Amount > availableAmount)
            {
                throw new Exception(
                    "CAPEX request amount exceeds the available budget.");
            }

            CapexRequest data =mapper.Map<CapexRequest>(dto);
            data.RequestedBy = currentUser.UserId;
            data.IsActive = 1;
            data.Status = "Pending";
            data.ApprovedBy = null;
            data.ApprovedDate = null;
            //int userId = 1;
            data.CreatedBy = currentUser.UserId;
            data.CreatedAt = DateTime.Now;

            await db.CapexRequest.AddAsync(data);
            await db.SaveChangesAsync();

            var result = await db.CapexRequest
                .Include(x => x.BudgetLine)
                    .ThenInclude(x => x.BudgetCategory)
                .Include(x => x.BudgetLine)
                    .ThenInclude(x => x.Budget)
                        .ThenInclude(x => x.Department)
                .Include(x => x.RequestedByUser)
                .Include(x => x.ApprovedByUser)
                .FirstOrDefaultAsync(x =>
                    x.CapexRequestId == data.CapexRequestId);

            return mapper.Map<CapexReadDTO>(result);
        }

        public async Task<List<CapexReadDTO>> GetAll(PaginationDTO pagination)
        {
            var search = db.CapexRequest.Where(x => x.IsActive == 1).AsQueryable();

            if (!string.IsNullOrEmpty(pagination.Search))
            {
                var keyword = pagination.Search.Trim();

                search = search.Where(x =>
                    x.Title.Contains(keyword) ||
                    x.Status.Contains(keyword));
                   
            }

            var user = await db.User
                .FirstOrDefaultAsync(x =>
                    x.UserId == currentUser.UserId);

            if (user == null)
            {
                throw new Exception("User not found.");
            }


            var data = await search
                .Include(x => x.BudgetLine)
                    .ThenInclude(x => x.BudgetCategory)
                .Include(x => x.BudgetLine)
                    .ThenInclude(x => x.Budget)
                        .ThenInclude(x => x.Department)
                .Include(x => x.RequestedByUser)
                .Include(x => x.ApprovedByUser)
                .Where(x => x.RequestedBy == currentUser.UserId)
                .OrderByDescending(x => x.CapexRequestId)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return mapper.Map<List<CapexReadDTO>>(data);
        }

        public async Task<CapexReadDTO?> GetById(int capexRequestId)
        {
            var data = await db.CapexRequest
                .Include(x => x.BudgetLine)
                    .ThenInclude(x => x.BudgetCategory)
                .Include(x => x.BudgetLine)
                    .ThenInclude(x => x.Budget)
                        .ThenInclude(x => x.Department)
                .Include(x => x.RequestedByUser)
                .Include(x => x.ApprovedByUser)
                .FirstOrDefaultAsync(x =>
                    x.CapexRequestId == capexRequestId &&
    x.IsActive == 1);

            if (data == null)
            {
                throw new Exception("CAPEX request not found.");
            }

            return mapper.Map<CapexReadDTO>(data);
        }

        public async Task<bool> UpdateCapexRequest(int capexRequestId,CapexWriteDTO dto)
        {
            var capex = await db.CapexRequest
                .FirstOrDefaultAsync(x => x.CapexRequestId == capexRequestId &&
    x.IsActive == 1);

            if (capex == null)
            {
                throw new Exception("CAPEX request not found.");
            }

            if (capex.Status != "Pending")
            {
                throw new Exception("Only pending CAPEX requests can be updated.");
            }

            if (capex.RequestedBy != currentUser.UserId)
            {
                throw new Exception("You can update only your own CAPEX request.");
            }

            var budgetLine = await db.BudgetLine
                .FirstOrDefaultAsync(x => x.BudgetLineId == dto.BudgetLineId &&
    x.IsActive == 1);

            if (budgetLine == null)
            {
                throw new Exception("Budget line not found.");
            }

            decimal approvedAmount = await db.CapexRequest
                .Where(x =>
                    x.BudgetLineId == dto.BudgetLineId &&
                    x.Status == "Approved" && x.IsActive == 1)
                .SumAsync(x => (decimal?)x.Amount) ?? 0;

            decimal availableAmount = budgetLine.AllocatedAmount - approvedAmount;

            if (dto.Amount > availableAmount)
            {
                throw new Exception("CAPEX amount exceeds available budget.");
            }

            capex.BudgetLineId = dto.BudgetLineId;
            capex.Title = dto.Title;
            capex.Amount = dto.Amount;
            //int userId = 1;
            capex.ModifiedBy = currentUser.UserId;
            capex.ModifiedAt = DateTime.Now;
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCapexRequest(int capexRequestId)
        {
            var capex = await db.CapexRequest
                .FirstOrDefaultAsync(x => x.CapexRequestId == capexRequestId &&
    x.IsActive == 1);

            if (capex == null)
            {
                throw new Exception("CAPEX request not found.");
            }

            if (capex.RequestedBy != currentUser.UserId)
            {
                throw new Exception("You can delete only your own CAPEX request.");
            }

            if (capex.Status != "Pending")
            {
                throw new Exception("Only pending CAPEX requests can be deleted.");
            }
            capex.IsActive = 0;
            capex.ModifiedBy = currentUser.UserId;
            capex.ModifiedAt = DateTime.Now;
            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> VerifyCapexRequest(int capexRequestId,CapexVerifyDTO dto)
        {
            var capex = await db.CapexRequest
                .Include(x => x.BudgetLine)
                    .ThenInclude(x => x.Budget)
                .FirstOrDefaultAsync(x =>
                    x.CapexRequestId == capexRequestId &&
                    x.IsActive == 1);

            if (capex == null)
            {
                throw new Exception("CAPEX request not found.");
            }

            var approver = await db.User
                .FirstOrDefaultAsync(x =>
                    x.UserId == currentUser.UserId &&
                    x.IsActive == 1);

            if (approver == null)
            {
                throw new Exception("Approver not found.");
            }

            if (capex.Status != "Pending")
            {
                throw new Exception(
                    "CAPEX request is already verified.");
            }

            //if (dto.Status != "Approved" &&
            //    dto.Status != "Rejected")
            //{
            //    throw new Exception(
            //        "Status must be Approved or Rejected.");
            //}
            var status = dto.Status.Trim();

            if (!status.Equals("Approved", StringComparison.OrdinalIgnoreCase) &&
                !status.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Status must be Approved or Rejected.");
            }

            var approval = await db.Approval
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x =>
                    x.IsActive == 1 &&
                    capex.Amount >= x.MinAmount &&
                    capex.Amount <= x.MaxAmount);

            if (approval == null)
            {
                throw new Exception(
                    "Approval rule not found for this CAPEX amount.");
            }

            if (approver.RoleId != approval.RoleId)
            {
                throw new Exception(
                    "You cannot verify this CAPEX request. " +
                    "It must be verified by the " +
                    approval.Role.RoleName + ".");
            }

            if (dto.Status == "Approved")
            {
                decimal approvedAmount = await db.CapexRequest
                    .Where(x =>
                        x.BudgetLineId == capex.BudgetLineId &&
                        x.Status == "Approved" &&
                        x.IsActive == 1)
                    .SumAsync(x => (decimal?)x.Amount) ?? 0;

                decimal availableAmount =
                    capex.BudgetLine.AllocatedAmount -
                    approvedAmount;

                if (capex.Amount > availableAmount)
                {
                    throw new Exception(
                        "Insufficient available budget.");
                }

                bool prExists = await db.PurchaseRequisition
                    .AnyAsync(x =>
                        x.CapexRequestId == capex.CapexRequestId &&
                        x.IsActive == 1);

                if (prExists)
                {
                    throw new Exception(
                        "Purchase Requisition already exists " +
                        "for this CAPEX request.");
                }

                var pr = new PurchaseRequisition
                {
                    CapexRequestId = capex.CapexRequestId,

                    PRNumber =
                        "PR-" +
                        DateTime.Now.ToString("yyyyMMddHHmmssfff"),

                    Title = capex.Title,

                    Description =
                        "Created automatically from CAPEX request.",

                    Status = "Pending",

                    IsActive = 1,
                    CreatedBy = currentUser.UserId,
                    CreatedAt = DateTime.Now
                };

                await db.PurchaseRequisition.AddAsync(pr);

                capex.Status = "Approved";
                capex.ApprovedBy = currentUser.UserId;
                capex.ApprovedDate = DateTime.Now;
            }
            else
            {
                capex.Status = "Rejected";
                capex.ApprovedBy = null;
                capex.ApprovedDate = null;
            }

            capex.ModifiedBy = currentUser.UserId;
            capex.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();

            return true;
        }
        public async Task<int> GetTotalRecord()
        {
            return await db.CapexRequest.CountAsync(x => x.IsActive == 1);
            //return await db.CapexRequest.CountAsync(x => x.RequestedBy == currentUser.UserId);
        }
    }
}
